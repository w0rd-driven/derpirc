using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using derpirc.Data.Settings;
using IrcDotNet;
using derpirc.Data;
using System.Threading;

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

        // EFNet: Welcome to the $network Internet Relay Chat Network $nick
        // PowerPrecision: Welcome to the $network IRC Network $nick!$email@$host
        private Regex welcomeRegex = new Regex("^.*?Welcome to the (.*?) (IRC|Internet Relay Chat) Network (.*)", RegexOptions.Compiled);

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
            _clients.Add(InitializeClient());
            _clients.Add(InitializeClient());
            // TODO: Database
            //_unitOfWork = new DataUnitOfWork();
            //_unitOfWork.InitializeDatabase();
            var session = _unitOfWork.Sessions.FindBy(x => x.Name == "Default").FirstOrDefault();
            _session = session;
        }

        public void Connect()
        {
            var registrationData = GetRegistrationInfo();
            for (int index = 0; index < _clients.Count; index++)
            {
                var client = _clients[index];
                client.ClientId = (index + 1).ToString();
                var server = GetServer(index + 1);
                client.Connect(server.HostName, server.Port, false, registrationData);
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
            _clientStates.Add(0);
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
            throw new NotImplementedException();
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
            client.LocalUser.JoinedChannel += LocalUser_JoinedChannel;
            client.LocalUser.LeftChannel += LocalUser_LeftChannel;
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
            var localUser = (IrcLocalUser)sender;
            //OnLocalUserNickNameChanged(localUser, e);
        }

        private void LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;
            //OnLocalUserNoticeReceived(localUser, e);
        }

        private void LocalUser_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;
            if (e.Source is IrcUser)
            {
                // Read message and process if it is chat command.
                //if (ReadChatCommand(localUser.Client, e))
                    return;
            }
            //OnLocalUserMessageReceived(localUser, e);
        }

        private void LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;
            e.Channel.UserJoined += Channel_UserJoined;
            e.Channel.UserLeft += Channel_UserLeft;
            e.Channel.MessageReceived += Channel_MessageReceived;
            e.Channel.NoticeReceived += Channel_NoticeReceived;
            //OnLocalUserJoinedChannel(localUser, e);
        }

        private void LocalUser_LeftChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;
            e.Channel.UserJoined -= Channel_UserJoined;
            e.Channel.UserLeft -= Channel_UserLeft;
            e.Channel.MessageReceived -= Channel_MessageReceived;
            e.Channel.NoticeReceived -= Channel_NoticeReceived;
            //OnLocalUserJoinedChannel(localUser, e);
        }

        private void Channel_UserLeft(object sender, IrcChannelUserEventArgs e)
        {
            var channel = (IrcChannel)sender;
            //OnChannelUserJoined(channel, e);
        }

        private void Channel_UserJoined(object sender, IrcChannelUserEventArgs e)
        {
            var channel = (IrcChannel)sender;
            //OnChannelUserLeft(channel, e);
        }

        private void Channel_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = (IrcChannel)sender;
            //OnChannelNoticeReceived(channel, e);
        }

        private void Channel_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = (IrcChannel)sender;
            if (e.Source is IrcUser)
            {
                // Read message and process if it is chat command.
                //if (ReadChatCommand(channel.Client, e))
                    return;
            }
            //OnChannelMessageReceived(channel, e);
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
                JoinSession(matchedServer, client);
            }
        }

        private SessionServer GetServer(int clientId)
        {
            return _session.Servers.FirstOrDefault(x => x.BasedOnId == clientId); ;
        }

        private SessionServer GetServer(int clientId, string serverName)
        {
            var result = _session.Servers.FirstOrDefault(x => x.BasedOnId == clientId);
            if (result != null)
            {
                result.HostName = serverName;
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
                result.Name = networkName;
            }
            return result;
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