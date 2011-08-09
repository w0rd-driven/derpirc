using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using derpirc.Data;
using derpirc.Data.Settings;
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
        private IList<int> _clientStates;
        private IrcRegistrationInfo _registrationData;

        private ChannelsSupervisor _channelSupervisor;

        // HACK: UI facing
        public event EventHandler<ChannelStatusEventArgs> ChannelJoined;
        public event EventHandler<ChannelStatusEventArgs> ChannelLeft;
        public event EventHandler<ChannelItemEventArgs> MessageReceived;
        public event EventHandler<MentionItemEventArgs> MentionReceived;

        // EFNet: Welcome to the $server Internet Relay Chat Network $nick
        // PowerPrecision: Welcome to the $server IRC Network $nick!$email@$host
        private static readonly Regex welcomeRegex = new Regex("^.*?Welcome to the (.*?) (IRC|Internet Relay Chat) Network (.*)", RegexOptions.Compiled);

        public SessionSupervisor(DataUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _clients = new ObservableCollection<IrcClient>();
            _clientStates = new List<int>();
            Initialize();
            Connect();
        }

        public void Initialize()
        {
            // Assume database is at least populated with Default Factory settings

            // HACK: Test First Init
            _unitOfWork.InitializeDatabase(true);

            var session = _unitOfWork.Sessions.FindBy(x => x.Name == "Default").FirstOrDefault();
            _session = session;
            var servers = _session.Servers.ToList();
            servers.ForEach(item =>
            {
                var serverName = string.Empty;
                serverName = item.HostName;
                // HACK: Fix up special Factory.Create case
                if (item.Server == null)
                {
                    var basedOnServer = GetBasedOnServer(serverName);
                    if (basedOnServer != null)
                    {
                        item.Server = basedOnServer;
                        item.BasedOnId = basedOnServer.Id;
                    }
                }
                var client = InitializeClient();
                client.ClientId = item.Id.ToString();
                _clients.Add(client);
            });

            // HACK: LazyLoad this somewhere else and inject the dependency
            _unitOfWork.Commit();
            _channelSupervisor = new ChannelsSupervisor(_unitOfWork, _session);
            _channelSupervisor.ChannelJoined += new EventHandler<ChannelStatusEventArgs>(_channelSupervisor_ChannelJoined);
            _channelSupervisor.ChannelLeft += new EventHandler<ChannelStatusEventArgs>(_channelSupervisor_ChannelLeft);
            _channelSupervisor.MessageReceived += new EventHandler<ChannelItemEventArgs>(_channelSupervisor_MessageReceived);
            _channelSupervisor.MentionReceived += new EventHandler<MentionItemEventArgs>(_channelSupervisor_MentionReceived);
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

        void _channelSupervisor_MessageReceived(object sender, ChannelItemEventArgs e)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void _channelSupervisor_MentionReceived(object sender, MentionItemEventArgs e)
        {
            var handler = MentionReceived;
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

        public void SendMessage(ChannelSummary channel, string message)
        {
            //_channelSupervisor.SendMessage(channel, message);
        }

        private IrcUserRegistrationInfo GetRegistrationInfo()
        {
            // TODO: Database
            var sessionUser = Factory.CreateUser();
            _quitMessage = sessionUser.QuitMessage;
            var result = new IrcUserRegistrationInfo();
            result.NickName = sessionUser.NickName;
            result.RealName = sessionUser.Name;
            var userName = sessionUser.Email.Remove(sessionUser.Email.IndexOf("@"));
            result.UserName = userName;
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
            client.LocalUser.NickNameChanged += LocalUser_NickNameChanged;
            client.LocalUser.NoticeReceived += LocalUser_NoticeReceived;
            client.LocalUser.MessageReceived += LocalUser_MessageReceived;
            _channelSupervisor.LocalUsers.Add(client.LocalUser);
            _clientStates.Add(0);
            //ProcessSession(client);
            //OnClientRegistered(client);
        }

        void Client_NetworkInformationReceived(object sender, EventArgs e)
        {
            var client = sender as IrcClient;
            ProcessSession(client);
        }

        private void LocalUser_NickNameChanged(object sender, EventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            //OnLocalUserNickNameChanged(localUser, e);
        }

        private void LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            //OnLocalUserNoticeReceived(localUser, e);
        }

        private void LocalUser_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            if (e.Source is IrcUser)
            {
                // Read message and process if it is chat command.
                //if (ReadChatCommand(localUser.Client, e))
                    return;
            }
            //OnLocalUserMessageReceived(localUser, e);
        }

        private void ProcessSession(IrcClient client)
        {
            var clientId = -1;
            int.TryParse(client.ClientId, out clientId);
            var index = clientId - 1;
            if (_clientStates[index] == 0)
            {
                _clientStates[index] = 1;
                var matchedServer = GetServer(clientId, client.ServerName);
                var matchedNetwork = GetNetwork(client.WelcomeMessage);
                LinkSession(matchedServer, matchedNetwork);
                // TODO: Wire up settings UI call for IsAutoJoinSession
                JoinSession(matchedServer, client);
            }
        }

        private Server GetBasedOnServer(string hostName)
        {
            return _unitOfWork.Servers.FindBy(x => x.HostName == hostName).FirstOrDefault();
        }

        private SessionServer GetServer(string clientId)
        {
            var integerId = -1;
            int.TryParse(clientId, out integerId);
            // TODO: Error handling make sure _session != null
            // Also make sure session is created first either through Committing defaults or whatever need be.
            return _session.Servers.FirstOrDefault(x => x.Id == integerId);
        }

        private SessionServer GetServer(int clientId, string serverName)
        {
            // TODO: Error handling make sure _session != null
            var result = _session.Servers.FirstOrDefault(x => x.Id == clientId);
            if (result != null)
            {
                result.HostName = serverName.ToLower();
            }
            return result;
        }

        private SessionNetwork GetNetwork(string message)
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

        private User GetUser()
        {
            var sessionUser = Factory.CreateUser();
            //var sessionUser = _unitOfWork.User.FindBy(x => x.Name == channel.Name.ToLower()).FirstOrDefault();
            //if (sessionUser == null)
            //{
            //    sessionUser = Factory.CreateUser();
            //    _unitOfWork.Commit();
            //}
            return sessionUser;
        }

        private void LinkSession(SessionServer server, SessionNetwork network)
        {
            if (network != null)
            {
                server.NetworkId = network.Id;
                server.Network = network;
                _unitOfWork.Commit();
            }
        }

        private void JoinSession(SessionServer server, IrcClient client)
        {
            if (server.Network != null)
            {
                string[] separator = new string[] { ", " };
                var channels = server.Network.JoinChannels.Split(separator, StringSplitOptions.RemoveEmptyEntries);
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
