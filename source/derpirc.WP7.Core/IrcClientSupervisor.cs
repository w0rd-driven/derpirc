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
    public class IrcClientSupervisor : IDisposable
    {
        #region Properties

        private bool _isNetworkAvailable;
        public bool IsNetworkAvailable
        {
            get { return _isNetworkAvailable; }
            set { _isNetworkAvailable = value; }
        }

        #endregion

        public event EventHandler<ClientStatusEventArgs> StateChanged;

        private bool _isDisposed;
        private int _quitTimeout = 1000;
        private int _defaultServerPort = 6667;

        private DataUnitOfWork _unitOfWork;

        private object _threadLock = new object();
        private BackgroundWorker _worker;

        // EFNet: Welcome to the $server Internet Relay Chat Network $nick
        // PowerPrecision: Welcome to the $server IRC Network $nick!$email@$host
        private static readonly Regex _welcomeRegex = new Regex("^.*?Welcome to the (.*?) (IRC|Internet Relay Chat) Network (.*)", RegexOptions.Compiled);

        public IrcClientSupervisor(DataUnitOfWork unitOfWork)
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
                // Build Client list based on settings
                for (int index = 0; index < SettingsUnitOfWork.Default.Networks.Count; index++)
                {
                    var item = SettingsUnitOfWork.Default.Networks[index];
                    var client = this.InitializeClientItem();
                    client.Info.Id = index + 1;
                    client.Info.NetworkName = item.Name;
                    SupervisorFacade.Default.Clients.Add(client);
                }

                // TODO: SmartMonitor
                // HACK: Turned off while loop since it breaks first run sync
                //while (session == null)
                //{
                //    session = this.GetDefaultSession();
                //    Thread.Sleep(1000);
                //}

                var session = this.GetDefaultSession();

                if (session != null)
                {
                    this.Connect();
                }
            }

            if (completed != null)
            {
                completed();
            }
        }

        #region UI-facing methods

        private void Connect()
        {
            var clients = SupervisorFacade.Default.Clients.AsEnumerable();
            foreach (var item in clients)
            {
                this.Connect(item.Client);
            }
        }

        private void Connect(IrcClient client)
        {
            // TODO: SmartMonitor
            var network = this.GetNetworkByClient(client);
            if (network != null && client != null)
            {
                var registrationData = GetRegistrationInfo();
                if (registrationData != null)
                    client.Connect(network.HostName, this.GetServerPort(network), false, registrationData);
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
            try
            {
                var isConnected = item.Client.IsConnected;
                if (isConnected)
                {
                    item.Client.Quit(this._quitTimeout, SettingsUnitOfWork.Default.User.QuitMessage);
                    Thread.Sleep(_quitTimeout);
                }
            }
            catch (Exception exception)
            {
                // HACK: IrcClient.IsDisposed snafu
            }
            finally
            {
                var newClient = InitializeIrcClient();
                SupervisorFacade.Default.UpdateClient(item, newClient);
            }
        }

        public void Reconnect(bool force = false)
        {
            try
            {
                var clients = SupervisorFacade.Default.Clients.AsEnumerable();
                if (!force)
                    clients = clients.Where(x => !x.Client.IsConnected);

                foreach (var item in clients)
                {
                    Reconnect(item);
                }
            }
            catch (Exception exception)
            {
                // HACK: IrcClient.IsDisposed snafu
            }
        }

        public void Reconnect(ClientItem item)
        {
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

            if (SettingsUnitOfWork.Default.User != null)
            {
                result.NickName = SettingsUnitOfWork.Default.User.NickName;
                result.RealName = SettingsUnitOfWork.Default.User.FullName;
                result.UserName = SettingsUnitOfWork.Default.User.Username;
                if (SettingsUnitOfWork.Default.User.IsInvisible)
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
            UpdateState(client, ClientState.Disconnected, null);
            // Connect(client);
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            UpdateState(client, ClientState.Connected, null);
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            UpdateState(client, ClientState.Disconnected, null);
        }

        #endregion

        #region Failure events

        private void Client_ConnectFailed(object sender, IrcErrorEventArgs e)
        {
            var client = sender as IrcClient;
            UpdateState(client, ClientState.Error, e.Error);
        }

        private void Client_Error(object sender, IrcErrorEventArgs e)
        {
            var client = sender as IrcClient;
            if (e.Error.Message == "No such host is known")
            {
                UpdateState(client, ClientState.Intervention, e.Error);
                // TODO: Could not resolve hostname
            }
            else
                UpdateState(client, ClientState.Error, e.Error);
        }

        private void Client_ErrorMessageReceived(object sender, IrcErrorMessageEventArgs e)
        {
            var client = sender as IrcClient;
            if (e.Message == "Closing Link: ")
            {

            }
            // TODO: Intercept "Closing Link... " timeouts, not quits
        }

        private void Client_ProtocolError(object sender, IrcProtocolErrorEventArgs e)
        {
            var client = sender as IrcClient;
            var channelName = string.Empty;
            switch (e.Code)
            {
                case 464:
                    // Bad server password
                    break;
                case 465:
                    // Banned from server
                    break;
                case 433:
                    // Nick in use
                    NickCollision(client);
                    break;
                case 473:
                    // Invite only (and you're not invited :()
                    channelName = e.Parameters[0];
                    // TODO: Recovery : Channel not joined (due to +i)
                    break;
                case 474:
                    // TODO: Recovery : Channel not joined (due to +b)
                    channelName = e.Parameters[0];
                    // Banned
                    break;
                case 475:
                    // TODO: Recovery : Channel not joined (due to +k)
                    channelName = e.Parameters[0];
                    // Bad channel key
                    break;
                case 482:
                    // Not operator
                    break;
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
            UpdateState(client, ClientState.Registered, null);
        }

        private void Client_NetworkInformationReceived(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            this.ProcessSession(client);
        }

        private void CtcpClient_ActionReceived(object sender, CtcpMessageEventArgs e)
        {
            var client = sender as CtcpClient;
        }

        private void CtcpClient_ActionSent(object sender, CtcpMessageEventArgs e)
        {
            var client = sender as CtcpClient;
        }

        private void ProcessSession(IrcClient client)
        {
            var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);
            if (foundClient.Info.State == ClientState.Registered)
            {
                UpdateState(client, ClientState.Processed, null);
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
                var channels = network.Favorites.Where(x => x.IsAutoConnect == true)
                    .Select(x => new Tuple<string, string>(x.Name, x.Password)).AsEnumerable();
                if (channels.Any())
                    client.Channels.Join(channels);
            }
        }

        private void NickCollision(IrcClient client)
        {
            var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);

            var nickNameAlternate = string.Empty;
            if (foundClient.Info.NickNameRetryCount == 0)
                nickNameAlternate = SettingsUnitOfWork.Default.User.NickNameAlternate;
            else
            {
                var suffix = foundClient.Info.NickNameRetryCount.ToString();
                nickNameAlternate = client.LocalUser.NickName
                    .Substring(0, client.LocalUser.NickName.Length - suffix.Length) + suffix;
            }

            client.LocalUser.SetNickName(nickNameAlternate);
            foundClient.Info.NickNameRetryCount++;
        }

        private void UpdateState(IrcClient client, ClientState state, Exception error)
        {
            var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);
            if (foundClient != null)
            {
                foundClient.Info.State = state;
                foundClient.Info.Error = error;
                var eventArgs = new ClientStatusEventArgs()
                {
                    Info = foundClient.Info,
                };
                OnStateChanged(eventArgs);
            }
        }

        #region Events

        void OnStateChanged(ClientStatusEventArgs eventArgs)
        {
            var handler = this.StateChanged;
            if (handler != null)
                handler.Invoke(this, eventArgs);
        }

        #endregion

        #region Lookup methods

        public Session GetDefaultSession()
        {
            var result = this._unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
            return result;
        }

        public int GetServerPort(Network network)
        {
            int result = _defaultServerPort;
            if (network != null)
                int.TryParse(network.Ports, out result);
            return result;
        }

        public List<Network> GetNetworks()
        {
            List<Network> result;
            var session = GetDefaultSession();
            if (session != null)
                result = session.Networks.ToList();
            else
                result = null;
            return result;
        }

        public Network GetNetworkByName(string networkName)
        {
            Network result;
            var session = GetDefaultSession();
            if (session != null)
                result = session.Networks.FirstOrDefault(x => x.Name == networkName.ToLower());
            else
                result = null;
            return result;
        }

        public Network GetNetworkByClient(IrcClient client)
        {
            Network result;
            var session = GetDefaultSession();
            var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);
            if (session != null && foundClient != null)
                result = session.Networks.FirstOrDefault(x => x.Id == foundClient.Info.Id);
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
