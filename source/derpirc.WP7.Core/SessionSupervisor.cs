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
        private string _quitMessage;
        private DataUnitOfWork _unitOfWork;

        private Session _session;
        private IList<IrcClient> _clients;
        private IDictionary<string, int> _clientStates;
        private IrcRegistrationInfo _registrationData;

        private ChannelsSupervisor _channelSupervisor;
        private MessagesSupervisor _messageSupervisor;

        // HACK: UI facing
        public event EventHandler<ChannelStatusEventArgs> ChannelJoined;
        public event EventHandler<ChannelStatusEventArgs> ChannelLeft;
        public event EventHandler<MessageItemEventArgs> ChannelItemReceived;
        public event EventHandler<MessageItemEventArgs> MentionItemReceived;
        public event EventHandler<MessageItemEventArgs> MessageItemReceived;

        // EFNet: Welcome to the $server Internet Relay Chat Network $nick
        // PowerPrecision: Welcome to the $server IRC Network $nick!$email@$host
        private static readonly Regex welcomeRegex = new Regex("^.*?Welcome to the (.*?) (IRC|Internet Relay Chat) Network (.*)", RegexOptions.Compiled);

        public SessionSupervisor(DataUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _clients = new ObservableCollection<IrcClient>();
            _clientStates = new Dictionary<string, int>();
            Initialize();
            Connect();
        }

        public void Initialize()
        {
            // Assume database is at least populated with Default Factory settings

            // HACK: Test First Init
            //_unitOfWork.InitializeDatabase(true);
            DataUnitOfWork.Default.InitializeDatabase(true);

            //var session = _unitOfWork.Sessions.FindBy(x => x.Name == "Default").FirstOrDefault();
            var session = DataUnitOfWork.Default.Sessions.FindBy(x => x.Name == "Default").FirstOrDefault();
            _session = session;
            var networks = _session.Networks.ToList();
            networks.ForEach(item =>
            {
                var client = InitializeClient();
                client.ClientId = item.Id.ToString();
                _clients.Add(client);
            });

            // HACK: LazyLoad this somewhere else and inject the dependency
            //_unitOfWork.Commit();
            DataUnitOfWork.Default.Commit();
            _channelSupervisor = new ChannelsSupervisor(_unitOfWork, _session);
            _channelSupervisor.ChannelJoined += new EventHandler<ChannelStatusEventArgs>(_channelSupervisor_ChannelJoined);
            _channelSupervisor.ChannelLeft += new EventHandler<ChannelStatusEventArgs>(_channelSupervisor_ChannelLeft);
            _channelSupervisor.ChannelItemReceived += new EventHandler<MessageItemEventArgs>(_channelSupervisor_MessageReceived);
            _channelSupervisor.MentionItemReceived += new EventHandler<MessageItemEventArgs>(_channelSupervisor_MentionReceived);

            _messageSupervisor = new MessagesSupervisor(_unitOfWork, _session);
            _messageSupervisor.MessageItemReceived += new EventHandler<MessageItemEventArgs>(_messageSupervisor_MessageReceived);
        }

        void _channelSupervisor_ChannelJoined(object sender, ChannelStatusEventArgs e)
        {
            var handler = ChannelJoined;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void _channelSupervisor_ChannelLeft(object sender, ChannelStatusEventArgs e)
        {
            var handler = ChannelLeft;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void _channelSupervisor_MessageReceived(object sender, MessageItemEventArgs e)
        {
            var handler = ChannelItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void _channelSupervisor_MentionReceived(object sender, MessageItemEventArgs e)
        {
            var handler = MentionItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void _messageSupervisor_MessageReceived(object sender, MessageItemEventArgs e)
        {
            var handler = MessageItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        public void Connect()
        {
            _registrationData = GetRegistrationInfo();
            for (int index = 0; index < _clients.Count; index++)
            {
                var client = _clients[index];
                var server = GetServer(client.ClientId);
                client.Connect(server.HostName, server.Port, false, _registrationData);
            }
        }

        public void Disconnect()
        {
            var connectedClients = _clients.Where(x => x.IsConnected);
            foreach (var item in connectedClients)
            {
                item.Quit(_quitTimeout, _quitMessage);
            }
        }

        public void SendMessage(ChannelItem message)
        {
            _channelSupervisor.SendMessage(message);
        }

        public void SendMessage(MentionItem message)
        {
            _channelSupervisor.SendMessage(message);
        }

        public void SendMessage(MessageItem message)
        {
            _messageSupervisor.SendMessage(message);
        }

        private IrcUserRegistrationInfo GetRegistrationInfo()
        {
            // TODO: Database
            Data.Models.Settings.User sessionUser = Data.Models.Settings.Factory.CreateUser();
            _quitMessage = sessionUser.QuitMessage;
            var result = new IrcUserRegistrationInfo();
            result.NickName = sessionUser.NickName;
            result.RealName = sessionUser.FullName;
            var userName = sessionUser.Username;
            result.UserName = userName;
            if (sessionUser.IsInvisible)
            {
                result.UserModes = new Collection<char>();
                result.UserModes.Add('i');
            }
            return result;
        }

        private IrcClient InitializeClient()
        {
            var result = new IrcClient();
            result.FloodPreventer = new IrcStandardFloodPreventer(4, 2000);
            result.Connected += new EventHandler<EventArgs>(Client_Connected);
            result.ConnectFailed += new EventHandler<IrcErrorEventArgs>(Client_ConnectFailed);
            result.Disconnected += new EventHandler<EventArgs>(Client_Disconnected);
            result.Error += new EventHandler<IrcErrorEventArgs>(Client_Error);
            result.ErrorMessageReceived += new EventHandler<IrcErrorMessageEventArgs>(Client_ErrorMessageReceived);
            result.Registered += new EventHandler<EventArgs>(Client_Registered);
            result.NetworkInformationReceived += new EventHandler<EventArgs>(Client_NetworkInformationReceived);
            return result;
        }

        private void Client_Connected(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            // TODO: Pass through a standard OnConnected event
        }

        private void Client_ConnectFailed(object sender, IrcErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Client_Disconnected(object sender, EventArgs e)
        {
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
            // TODO: Intercept "Closing Link... " timeouts
        }

        private void Client_Registered(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            _channelSupervisor.LocalUsers.Add(client.LocalUser);
            _messageSupervisor.LocalUsers.Add(client.LocalUser);
            _clientStates.Add(client.ClientId, 0);
            //ProcessSession(client);
            //OnClientRegistered(client);
        }

        void Client_NetworkInformationReceived(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            ProcessSession(client);
        }

        private void ProcessSession(IrcClient client)
        {
            var keyValue = -1;
            var isParsed = false;
            isParsed = _clientStates.TryGetValue(client.ClientId, out keyValue);
            if (keyValue == 0)
            {
                _clientStates[client.ClientId] = 1;
                var clientId = -1;
                int.TryParse(client.ClientId, out clientId);
                var matchedServer = GetServer(clientId, client.ServerName);
                var matchedNetwork = GetNetwork(client.WelcomeMessage);
                // Commit the hostname and network casing change if necessary
                //_unitOfWork.Commit();
                DataUnitOfWork.Default.Commit();
                // TODO: Wire up settings UI call for IsAutoJoinSession
                JoinSession(matchedNetwork, client);
            }
        }

        private Server GetServer(string clientId)
        {
            var integerId = -1;
            int.TryParse(clientId, out integerId);
            // TODO: make sure session is created first either through Committing defaults or whatever need be.
            var result = new Server();
            if (_session != null)
            {
                var network = _session.Networks.FirstOrDefault(x => x.Id == integerId);
                result = network.Server;
            }
            return result;
        }

        private Server GetServer(int clientId, string serverName)
        {
            // TODO: Error handling make sure _session != null
            var result = GetServer(clientId.ToString());
            if (result != null)
            {
                result.HostName = serverName.ToLower();
            }
            return result;
        }

        private Network GetNetwork(string message)
        {
            var found = welcomeRegex.Match(message);
            var networkName = found.Groups[1].Value.TrimEnd();

            var myIdent = found.Groups[3].Value.TrimEnd();
            var nickName = (myIdent.IndexOf('!') > -1) ? myIdent.Substring(0, myIdent.IndexOf('!')) : myIdent;

            var result = _session.Networks.FirstOrDefault(x => x.Name.ToLower() == networkName.ToLower());
            if (result != null)
            {
                result.Name = networkName.ToLower();
            }
            return result;
        }

        private void JoinSession(Network network, IrcClient client)
        {
            if (network != null)
            {
                string[] separator = new string[] { ", " };
                var channels = network.JoinChannels.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                client.Channels.Join(channels);
            }
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
