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

namespace derpirc.Core
{
    public class SessionSupervisor : IDisposable
    {
        private bool _isDisposed;
        private const int _quitTimeout = 1000;

        private DataUnitOfWork _unitOfWork;
        private SettingsUnitOfWork _unitOfWorkSettings;

        private Session _session;
        private Data.Models.Settings.User _settings;

        private IrcRegistrationInfo _registrationData;

        private bool _isNetworkAvailable;
        public bool IsNetworkAvailable
        {
            get { return _isNetworkAvailable; }
            set { _isNetworkAvailable = value; }
        }

        private object _threadLock = new object();
        private BackgroundWorker _worker;

        // EFNet: Welcome to the $server Internet Relay Chat Network $nick
        // PowerPrecision: Welcome to the $server IRC Network $nick!$email@$host
        private static readonly Regex welcomeRegex = new Regex("^.*?Welcome to the (.*?) (IRC|Internet Relay Chat) Network (.*)", RegexOptions.Compiled);

        public SessionSupervisor(DataUnitOfWork unitOfWork, SettingsUnitOfWork unitOfWorkSettings)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkSettings = unitOfWorkSettings;

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(DeferStartupWork);

            DeferStartup(null);

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
            _worker.RunWorkerAsync(completed);
        }

        private void DeferStartupWork(object sender, DoWorkEventArgs e)
        {
            Action completed = e.Argument as Action;
            lock (_threadLock)
            {
                var session = GetDefaultSession();

                // TODO: SmartMonitor
                while (session == null)
                {
                    session = GetDefaultSession();
                    Thread.Sleep(1000);
                }

                if (session != null)
                {
                    _session = session;
                    var networks = _session.Networks;
                    foreach (var item in networks)
                    {
                        var client = InitializeClient();
                        client.Info.Id = item.Id;
                        client.Info.NetworkName = item.Name;
                        SupervisorFacade.Default.Clients.Add(client);
                    }
                    Connect();
                }
            }

            if (completed != null)
            {
                completed();
            }
        }

        public void Connect()
        {
            var clients = SupervisorFacade.Default.Clients.AsEnumerable();
            foreach (var item in clients)
            {
                Connect(item.Client);
            }
        }

        public void Connect(IrcClient client)
        {
            // TODO: SmartMonitor
            if (_registrationData == null)
                _registrationData = GetRegistrationInfo();
            if (_registrationData != null)
            {
                var server = GetServer(client);
                var serverPort = GetServerPort(server);
                if (client != null)
                    client.Connect(server.HostName, serverPort, false, _registrationData);
            }
        }

        public void Disconnect()
        {
            var clients = SupervisorFacade.Default.Clients.Where(x => x.Client.IsConnected);
            foreach (var item in clients)
            {
                Disconnect(item.Client);
            }
        }

        public void Disconnect(IrcClient client)
        {
            if (client.IsConnected)
                client.Quit(_quitTimeout, _settings.QuitMessage);
        }

        public void Reconnect(bool force)
        {
            var clients = SupervisorFacade.Default.Clients.AsEnumerable();
            if (!force)
                clients = clients.Where(x => !x.Client.IsConnected);

            foreach (var item in clients)
            {
                Connect(item.Client);
            }
        }

        private IrcUserRegistrationInfo GetRegistrationInfo()
        {
            var result = new IrcUserRegistrationInfo();

            _settings = _unitOfWorkSettings.User.FindBy(x => x.Name == "default").FirstOrDefault();
            if (_settings != null)
            {
                result.NickName = _settings.NickName;
                result.RealName = _settings.FullName;
                var userName = _settings.Username;
                result.UserName = userName;
                if (_settings.IsInvisible.HasValue && _settings.IsInvisible.Value)
                {
                    result.UserModes = new Collection<char>();
                    result.UserModes.Add('i');
                }
                return result;
            }
            else
                return null;
        }

        private ClientItem InitializeClient()
        {
            var result = new ClientItem();
            result.Client.FloodPreventer = new IrcStandardFloodPreventer(4, 2000);
            result.Client.Connected += new EventHandler<EventArgs>(Client_Connected);
            result.Client.ConnectFailed += new EventHandler<IrcErrorEventArgs>(Client_ConnectFailed);
            result.Client.Disconnected += new EventHandler<EventArgs>(Client_Disconnected);
            result.Client.Error += new EventHandler<IrcErrorEventArgs>(Client_Error);
            result.Client.ErrorMessageReceived += new EventHandler<IrcErrorMessageEventArgs>(Client_ErrorMessageReceived);
            result.Client.Registered += new EventHandler<EventArgs>(Client_Registered);
            result.Client.NetworkInformationReceived += new EventHandler<EventArgs>(Client_NetworkInformationReceived);
            result.Client.ServerBounce += new EventHandler<IrcServerInfoEventArgs>(Client_Bounce);
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

        #endregion

        private void Client_Registered(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            SupervisorFacade.Default.UpdateStatus(client, ClientState.Registered, null);
        }

        private void Client_NetworkInformationReceived(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            ProcessSession(client);
        }

        private void ProcessSession(IrcClient client)
        {
            var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);
            if (foundClient.Info.State == ClientState.Registered)
            {
                SupervisorFacade.Default.UpdateStatus(client, ClientState.Processed, null);
                var networkName = ParseNetworkName(client.WelcomeMessage);
                var matchedNetwork = GetNetworkByName(networkName);
                JoinSession(matchedNetwork, client);

                // Change local servername to match for easy reconnects
                //var matchedServer = GetServer(client, client.ServerName);
            }
        }

        private string ParseNetworkName(string message)
        {
            var result = string.Empty;
            var found = welcomeRegex.Match(message);
            var networkName = found.Groups[1].Value.TrimEnd();

            var myIdent = found.Groups[3].Value.TrimEnd();
            var nickName = (myIdent.IndexOf('!') > -1) ? myIdent.Substring(0, myIdent.IndexOf('!')) : myIdent;

            result = networkName;
            return result;
        }

        private void JoinSession(Network network, IrcClient client)
        {
            if (network != null && network.IsJoinEnabled.HasValue && network.IsJoinEnabled.Value)
            {
                string[] separator = new string[] { ", " };
                var channels = network.JoinChannels.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                client.Channels.Join(channels);
            }
        }

        #region Lookup methods

        public Session GetDefaultSession()
        {
            var result = _unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
            return result;
        }

        public Server GetServer(IrcClient client)
        {
            Server result;
            if (_session != null && _session.Networks != null)
            {
                var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);
                var network = _session.Networks.FirstOrDefault(x => x.Id == foundClient.Info.Id);
                result = network.Server;
            }
            else
                result = null;
            return result;
        }

        public Server GetServer(IrcClient client, string serverName)
        {
            var result = GetServer(client);
            if (result != null && result.HostName != serverName.ToLower())
            {
                result.HostName = serverName.ToLower();
                _unitOfWork.Commit();
            }
            return result;
        }

        public int GetServerPort(Server server)
        {
            int result = -1;
            if (server != null)
                int.TryParse(server.Ports, out result);
            return result;
        }

        public List<Network> GetNetworks()
        {
            List<Network> result;
            if (_session != null && _session.Networks != null)
                result = _session.Networks.ToList();
            else
                result = null;

            return result;
        }

        public Network GetNetworkByName(string networkName)
        {
            Network result;
            if (_session != null && _session.Networks != null)
                result = _session.Networks.FirstOrDefault(x => x.Name == networkName.ToLower());
            else
                result = null;
            return result;
        }

        public Network GetNetworkByClient(IrcClient client)
        {
            Network result;
            if (_session != null && _session.Networks != null)
            {
                var foundClient = SupervisorFacade.Default.GetClientByIrcClient(client);
                var network = _session.Networks.FirstOrDefault(x => x.Id == foundClient.Info.Id);
                result = network;
            }
            else
                result = null;
            return result;
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    // Disconnect each client gracefully.
                    Disconnect();
                }
            }
            this._isDisposed = true;
        }
    }
}
