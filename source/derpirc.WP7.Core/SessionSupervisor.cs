using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;
using IrcDotNet.Ctcp;

namespace derpirc.Core
{
    public class SessionSupervisor : IDisposable
    {
        #region Properties

        private bool _isNetworkAvailable;
        public bool IsNetworkAvailable
        {
            get { return _isNetworkAvailable; }
            set { _isNetworkAvailable = value; }
        }

        #endregion

        private bool _isDisposed;
        private int _quitTimeout = 1000;

        private DataUnitOfWork _unitOfWork;

        private Session _session;
        private Data.Models.Settings.User _settings;
        private IrcRegistrationInfo _registrationData;

        private object _threadLock = new object();
        private BackgroundWorker _worker;

        // EFNet: Welcome to the $server Internet Relay Chat Network $nick
        // PowerPrecision: Welcome to the $server IRC Network $nick!$email@$host
        private static readonly Regex _welcomeRegex = new Regex("^.*?Welcome to the (.*?) (IRC|Internet Relay Chat) Network (.*)", RegexOptions.Compiled);

        public SessionSupervisor(DataUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._worker = new BackgroundWorker();
            this._worker.DoWork += new DoWorkEventHandler(DeferStartupWork);

            this.DeferStartup(null);

            // TODO: Move to method
            //SupervisorFacade.Default.NetworkStatusChanged += (s, e) =>
            //{
            //    IsNetworkAvailable = e.IsAvailable;
            //    if (!IsNetworkAvailable)
            //    {

            //    }
            //    else
            //    {

            //    }
            //};
        }

        internal void DeferStartup(Action completed)
        {
            this._worker.RunWorkerAsync(completed);
        }

        private void DeferStartupWork(object sender, DoWorkEventArgs e)
        {
            Action completed = e.Argument as Action;
            lock (this._threadLock)
            {
                var session = this.GetDefaultSession();

                // TODO: SmartMonitor
                while (session == null)
                {
                    session = this.GetDefaultSession();
                    Thread.Sleep(1000);
                }

                if (session != null)
                {
                    this._session = session;
                    var networks = this._session.Networks;
                    foreach (var item in networks)
                    {
                        var client = this.InitializeClientItem();
                        client.Info.Id = item.Id;
                        client.Info.NetworkName = item.Name;
                        SupervisorFacade.Default.Clients.Add(client);
                    }
                    this.Connect();
                }
            }

            if (completed != null)
            {
                completed();
            }
        }

        #region UI-facing methods

        public void Connect()
        {
            var clients = SupervisorFacade.Default.Clients.AsEnumerable();
            foreach (var item in clients)
            {
                this.Connect(item.Client);
            }
        }

        public void Connect(IrcClient client)
        {
            // TODO: SmartMonitor
            if (this._registrationData == null)
                this._registrationData = this.GetRegistrationInfo();
            if (this._registrationData != null)
            {
                var network = this.GetNetworkByClient(client);
                var serverPort = this.GetServerPort(network);
                if (client != null)
                    client.Connect(network.HostName, serverPort, false, this._registrationData);
            }
        }

        public void Disconnect()
        {
            var clients = SupervisorFacade.Default.Clients.Where(x => x.Client.IsConnected);
            foreach (var item in clients)
            {
                this.Disconnect(item);
            }
        }

        public void Disconnect(ClientItem item)
        {
            if (item.Client != null)
            {
                if (item.Client.IsConnected)
                    item.Client.Quit(this._quitTimeout, this._settings.QuitMessage);
                //if (!item.Client.IsConnected)
                //{
                //    var newClient = InitializeIrcClient();
                //    SupervisorFacade.Default.UpdateClient(item, newClient);
                //}
            }
        }

        public void Reconnect(bool force = false)
        {
            var clients = SupervisorFacade.Default.Clients.AsEnumerable();
            if (!force)
                clients = clients.Where(x => !x.Client.IsConnected);

            foreach (var item in clients)
            {
                if (item.Client.IsConnected)
                    this.Disconnect(item);
                this.Connect(item.Client);
            }
        }

        public void Reconnect(ClientItem item, bool force = false)
        {
            if (item.Client.IsConnected)
                this.Disconnect(item);
            this.Connect(item.Client);
        }

        public void SetNickName(IMessage target, string nickName)
        {
            if (target != null)
            {
                var localUser = SupervisorFacade.Default.GetLocalUserBySummary(target);
                if (localUser != null && !string.IsNullOrEmpty(nickName))
                    localUser.SetNickName(nickName);
            }
        }

        public void SendAction(IMessage target, string message)
        {
            if (target != null)
            {
                var client = SupervisorFacade.Default.GetCtcpClientBySummary(target);
                if (client != null)
                {
                    //client.SendAction(target.Name, message);
                }
            }
        }

        #endregion

        private IrcUserRegistrationInfo GetRegistrationInfo()
        {
            var result = new IrcUserRegistrationInfo();

            this._settings = SettingsUnitOfWork.Default.User;
            if (this._settings != null)
            {
                result.NickName = this._settings.NickName;
                result.RealName = this._settings.FullName;
                result.UserName = this._settings.Username;
                if (this._settings.IsInvisible)
                {
                    result.UserModes = new Collection<char>();
                    result.UserModes.Add('i');
                }
                return result;
            }
            else
                return null;
        }

        private ClientItem InitializeClientItem()
        {
            var result = new ClientItem();

            result.Client = InitializeIrcClient();
            result.CtcpClient = InitializeCtcpClient(result.Client);

            return result;
        }

        private IrcClient InitializeIrcClient()
        {
            var result = new IrcClient();
            result.FloodPreventer = new IrcStandardFloodPreventer(4, 2000);
            // Connection events
            result.Connected += new EventHandler<EventArgs>(Client_Connected);
            result.Disconnected += new EventHandler<EventArgs>(Client_Disconnected);
            result.ServerBounce += new EventHandler<IrcServerInfoEventArgs>(Client_Bounce);

            // Failure events
            result.ConnectFailed += new EventHandler<IrcErrorEventArgs>(Client_ConnectFailed);
            result.ProtocolError += new EventHandler<IrcProtocolErrorEventArgs>(Client_ProtocolError);
            result.Error += new EventHandler<IrcErrorEventArgs>(Client_Error);
            result.ErrorMessageReceived += new EventHandler<IrcErrorMessageEventArgs>(Client_ErrorMessageReceived);

            // Detection events
            result.Registered += new EventHandler<EventArgs>(Client_Registered);
            result.NetworkInformationReceived += new EventHandler<EventArgs>(Client_NetworkInformationReceived);
            return result;
        }

        private CtcpClient InitializeCtcpClient(IrcClient client)
        {
            var result = new CtcpClient(client);
            result.Error += new EventHandler<IrcErrorEventArgs>(CtcpClient_Error);
            result.ErrorMessageReceived += new EventHandler<CtcpErrorMessageReceivedEventArgs>(CtcpClient_ErrorMessageReceived);
            result.ActionReceived += new EventHandler<CtcpMessageEventArgs>(CtcpClient_ActionReceived);
            result.ActionSent += new EventHandler<CtcpMessageEventArgs>(CtcpClient_ActionSent);
            return result;
        }

        #region Connection events

        private void Client_Bounce(object sender, IrcServerInfoEventArgs e)
        {
            var client = sender as IrcClient;
            SupervisorFacade.Default.UpdateStatus(client, ClientState.Disconnected, null);
            // Connect(client);
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            SupervisorFacade.Default.UpdateStatus(client, ClientState.Connected, null);
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            SupervisorFacade.Default.UpdateStatus(client, ClientState.Disconnected, null);
        }

        #endregion

        #region Failure events

        private void Client_ConnectFailed(object sender, IrcErrorEventArgs e)
        {
            var client = sender as IrcClient;
            SupervisorFacade.Default.UpdateStatus(client, ClientState.Error, e.Error);
        }

        private void Client_Error(object sender, IrcErrorEventArgs e)
        {
            var client = sender as IrcClient;
            if (e.Error.Message == "No such host is known")
            {
                SupervisorFacade.Default.UpdateStatus(client, ClientState.Intervention, e.Error);
                // TODO: Could not resolve hostname
            }
            else
                SupervisorFacade.Default.UpdateStatus(client, ClientState.Error, e.Error);
        }

        private void Client_ErrorMessageReceived(object sender, IrcErrorMessageEventArgs e)
        {
            var client = sender as IrcClient;
            // TODO: Intercept "Closing Link... " timeouts
        }

        private void Client_ProtocolError(object sender, IrcProtocolErrorEventArgs e)
        {
            var client = sender as IrcClient;
            if (e.Code == 433)
            {
                // Nick in use
            }
        }

        private void CtcpClient_Error(object sender, IrcErrorEventArgs e)
        {
            var client = sender as CtcpClient;
        }

        private void CtcpClient_ErrorMessageReceived(object sender, IrcDotNet.Ctcp.CtcpErrorMessageReceivedEventArgs e)
        {
            var client = sender as CtcpClient;
        }

        #endregion

        private void Client_Registered(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            SupervisorFacade.Default.UpdateStatus(client, ClientState.Registered, null);
        }

        private void Client_NetworkInformationReceived(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            this.ProcessSession(client);
        }

        private void CtcpClient_ActionReceived(object sender, CtcpMessageEventArgs e)
        {
        }

        private void CtcpClient_ActionSent(object sender, CtcpMessageEventArgs e)
        {
        }

        private void ProcessSession(IrcClient client)
        {
            var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);
            if (foundClient.Info.State == ClientState.Registered)
            {
                SupervisorFacade.Default.UpdateStatus(client, ClientState.Processed, null);
                var networkName = this.ParseNetworkName(client.WelcomeMessage);
                var matchedNetwork = this.GetNetworkByName(networkName);
                this.JoinSession(matchedNetwork, client);

                // Change local servername to match for easy reconnects
                //var matchedServer = GetServer(client, client.ServerName);
            }
        }

        private string ParseNetworkName(string message)
        {
            var result = string.Empty;
            var found = _welcomeRegex.Match(message);
            var networkName = found.Groups[1].Value.TrimEnd();

            var myIdent = found.Groups[3].Value.TrimEnd();
            var nickName = (myIdent.IndexOf('!') > -1) ? myIdent.Substring(0, myIdent.IndexOf('!')) : myIdent;

            result = networkName;
            return result;
        }

        private void JoinSession(Network network, IrcClient client)
        {
            if (network != null)
            {
                var channels = network.Favorites.Where(x => x.IsAutoConnect == true).Select(x => x.Name).AsEnumerable();
                if (channels.Any())
                    client.Channels.Join(channels);
            }
        }

        #region Lookup methods

        public Session GetDefaultSession()
        {
            var result = this._unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
            return result;
        }

        public int GetServerPort(Network network)
        {
            int result = -1;
            if (network != null)
                int.TryParse(network.Ports, out result);
            return result;
        }

        public List<Network> GetNetworks()
        {
            List<Network> result;
            if (this._session != null && this._session.Networks != null)
                result = this._session.Networks.ToList();
            else
                result = null;

            return result;
        }

        public Network GetNetworkByName(string networkName)
        {
            Network result;
            if (this._session != null && this._session.Networks != null)
                result = this._session.Networks.FirstOrDefault(x => x.Name == networkName.ToLower());
            else
                result = null;
            return result;
        }

        public Network GetNetworkByClient(IrcClient client)
        {
            Network result;
            if (this._session != null && this._session.Networks != null)
            {
                var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);
                var network = this._session.Networks.FirstOrDefault(x => x.Id == foundClient.Info.Id);
                result = network;
            }
            else
                result = null;
            return result;
        }

        #endregion

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    // Disconnect each client gracefully.
                    this.Disconnect();
                }
            }
            this._isDisposed = true;
        }
    }
}
