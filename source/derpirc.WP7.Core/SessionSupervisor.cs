using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;

namespace derpirc.Core
{
    public class SessionSupervisor : IDisposable
    {
        private bool _isDisposed;
        private const int _quitTimeout = 1000;

        private Session _session;
        private Data.Models.Settings.User _settings;

        private IrcRegistrationInfo _registrationData;

        // EFNet: Welcome to the $server Internet Relay Chat Network $nick
        // PowerPrecision: Welcome to the $server IRC Network $nick!$email@$host
        private static readonly Regex welcomeRegex = new Regex("^.*?Welcome to the (.*?) (IRC|Internet Relay Chat) Network (.*)", RegexOptions.Compiled);

        public SessionSupervisor()
        {
        }

        public void Initialize()
        {
            var session = DataUnitOfWork.Default.Sessions.FindBy(x => x.Name == "Default").FirstOrDefault();
            _session = session;
            var networks = _session.Networks.ToList();
            foreach (var item in networks)
            {
                var client = InitializeClient();
                client.Id = item.Id;
                SupervisorFacade.Default.Clients.Add(client);
            }
            Connect();
        }

        public void Connect()
        {
            _registrationData = GetRegistrationInfo();
            foreach (var item in SupervisorFacade.Default.Clients)
            {
                var server = GetServer(item.Client);
                if (item.Client != null)
                    item.Client.Connect(server.HostName, server.Port, false, _registrationData);
            }
        }

        public void Disconnect()
        {
            var connectedClients = SupervisorFacade.Default.Clients.Where(x => x.Client.IsConnected);
            foreach (var item in connectedClients)
            {
                item.Client.Quit(_quitTimeout, _settings.QuitMessage);
            }
        }

        private IrcUserRegistrationInfo GetRegistrationInfo()
        {
            // TODO: Database lookup
            _settings = Data.Models.Settings.Factory.CreateUser();
            var result = new IrcUserRegistrationInfo();
            result.NickName = _settings.NickName;
            result.RealName = _settings.FullName;
            var userName = _settings.Username;
            result.UserName = userName;
            if (_settings.IsInvisible)
            {
                result.UserModes = new Collection<char>();
                result.UserModes.Add('i');
            }
            return result;
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
            return result;
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            // TODO: Pass through a standard OnConnected event
        }

        private void Client_ConnectFailed(object sender, IrcErrorEventArgs e)
        {
            var client = sender as IrcClient;
            // TODO: Pass through a standard OnConnectFailed event
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            // TODO: Pass through a standard OnDisconnected event
        }

        private void Client_Error(object sender, IrcErrorEventArgs e)
        {
            var client = sender as IrcClient;
            if (e.Error.Message == "No such host is known")
            {
                // TODO: Could not resolve hostname
            }
        }

        private void Client_ErrorMessageReceived(object sender, IrcErrorMessageEventArgs e)
        {
            var client = sender as IrcClient;
            // TODO: Intercept "Closing Link... " timeouts
        }

        private void Client_Registered(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            var foundClient = SupervisorFacade.Default.Clients.FirstOrDefault(x => x.Client == client);
            foundClient.State = ClientState.Registered;
            SupervisorFacade.Default.AttachLocalUser(client.LocalUser);
            //ProcessSession(client);
            //OnClientRegistered(client);
        }

        private void Client_NetworkInformationReceived(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            ProcessSession(client);
        }

        private void ProcessSession(IrcClient client)
        {
            var foundClient = SupervisorFacade.Default.Clients.FirstOrDefault(x => x.Client == client);
            if (foundClient.State == ClientState.Registered)
            {
                foundClient.State = ClientState.Processed;
                var networkName = ParseNetworkName(client.WelcomeMessage);
                var matchedNetwork = GetNetworkByName(networkName);
                // Commit the hostname and network casing change if necessary
                //_unitOfWork.Commit();
                // TODO: Wire up settings UI call for IsAutoJoinSession
                JoinSession(matchedNetwork, client);

                // Change local servername to match for easy reconnects
                var matchedServer = GetServer(client, client.ServerName);
                DataUnitOfWork.Default.Commit();
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
            if (network != null && network.IsJoinEnabled)
            {
                string[] separator = new string[] { ", " };
                var channels = network.JoinChannels.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                client.Channels.Join(channels);
            }
        }

        public Session GetDefaultSession()
        {
            var result = DataUnitOfWork.Default.Sessions.FindBy(x => x.Name == "Default").FirstOrDefault();
            return result;
        }

        public Server GetServer(IrcClient client)
        {
            Server result;
            if (_session != null && _session.Networks != null)
            {
                var foundClient = SupervisorFacade.Default.Clients.FirstOrDefault(x => x.Client == client);
                var network = _session.Networks.FirstOrDefault(x => x.Id == foundClient.Id);
                result = network.Server;
            }
            else
                result = null;
            return result;
        }

        public Server GetServer(IrcClient client, string serverName)
        {
            var result = GetServer(client);
            if (result != null)
                result.HostName = serverName.ToLower();
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
            {
                result = _session.Networks.FirstOrDefault(x => x.Name.ToLower() == networkName.ToLower());
                if (result != null)
                    result.Name = networkName.ToLower();
            }
            else
                result = null;
            return result;
        }

        public Network GetNetworkByClient(IrcClient client)
        {
            Network result;
            if (_session != null && _session.Networks != null)
            {
                var foundClient = SupervisorFacade.Default.Clients.FirstOrDefault(x => x.Client == client);
                var network = _session.Networks.FirstOrDefault(x => x.Id == foundClient.Id);
                result = network;
            }
            else
                result = null;
            return result;
        }

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
