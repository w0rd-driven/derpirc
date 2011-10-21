using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;
using Microsoft.Phone.Reactive;
using IrcDotNet.Ctcp;

namespace derpirc.Core
{
    public class SupervisorFacade : IDisposable
    {
        #region Properties

        private ObservableCollection<ClientItem> _clients;
        public ObservableCollection<ClientItem> Clients
        {
            get { return _clients; }
            set { _clients = value; }
        }

        private NetworkType _networkType;
        public NetworkType NetworkType
        {
            get { return _networkType; }
            set { _networkType = value; }
        }

        private bool _isNetworkAvailable;
        public bool IsNetworkAvailable
        {
            get { return _isNetworkAvailable; }
            set
            {
                if (_isNetworkAvailable == value)
                    return;

                _isNetworkAvailable = value;
            }
        }

        #endregion

        private bool _isDisposed;
        private object _threadLock = new object();
        private BackgroundWorker _worker;

        private DataUnitOfWork _unitOfWork;

        private NetworkMonitor _networkMonitor;
        private IDisposable _statusObserver;

        private SessionSupervisor _sessionSupervisor;
        private ChannelsSupervisor _channelSupervisor;
        private MessagesSupervisor _messageSupervisor;
        private SettingsSupervisor _settingSupervisor;

        // HACK: UI and internal facing
        public event EventHandler<NetworkStatusEventArgs> NetworkStatusChanged;

        // HACK: UI facing
        public event EventHandler<ClientStatusEventArgs> ClientStatusChanged;
        public event EventHandler<ChannelStatusEventArgs> ChannelJoined;
        public event EventHandler<ChannelStatusEventArgs> ChannelLeft;
        public event EventHandler<MessageItemEventArgs> ChannelItemReceived;
        public event EventHandler<MessageItemEventArgs> MentionItemReceived;
        public event EventHandler<MessageItemEventArgs> MessageItemReceived;

        #region Singleton Impl

        // Modified for http://www.yoda.arachsys.com/csharp/singleton.html #4. (Jon Skeet is a code machine)
        private static readonly SupervisorFacade defaultInstance = new SupervisorFacade();

        public static SupervisorFacade Default
        {
            get
            {
                return defaultInstance;
            }
        }

        static SupervisorFacade()
        {
        }

        #endregion

        public SupervisorFacade()
        {
            this._clients = new ObservableCollection<ClientItem>();

            this._worker = new BackgroundWorker();
            this._worker.DoWork += new DoWorkEventHandler(DeferStartupWork);

            this.DeferStartup(null);
        }

        internal void DeferStartup(Action completed)
        {
            this._worker.RunWorkerAsync(completed);
        }

        private void DeferStartupWork(object sender, DoWorkEventArgs e)
        {
            Action completed = e.Argument as Action;
            lock (_threadLock)
            {
                this.Startup();
            }

            if (completed != null)
            {
                completed();
            }
        }

        private void Startup()
        {
            this._networkMonitor = new NetworkMonitor(10000);
            this._statusObserver = _networkMonitor.Status()
                .ObserveOnDispatcher()
                .Subscribe(type =>
                {
                    this.NetworkType = type;
                    if (type != NetworkType.None)
                        this.IsNetworkAvailable = true;
                    else
                        this.IsNetworkAvailable = false;
                    var eventArgs = new NetworkStatusEventArgs()
                    {
                        IsAvailable = this.IsNetworkAvailable,
                        Type = type,
                    };
                    this.OnNetworkStatusChanged(this, eventArgs);
                });
            this._unitOfWork = new DataUnitOfWork();

            this._settingSupervisor = new SettingsSupervisor(_unitOfWork);
            this._sessionSupervisor = new SessionSupervisor(_unitOfWork);
        }

        private void Shutdown()
        {
            this._sessionSupervisor = null;
            this._channelSupervisor = null;
            this._messageSupervisor = null;
            this._settingSupervisor = null;
        }

        #region UI-facing methods

        // SettingsView
        public void CommitSettings()
        {
            this._settingSupervisor.Commit();
        }

        // ConnectionView
        public void Connect(ObservableCollection<ClientInfo> clients)
        {
            if (clients != null)
                foreach (var clientInfo in clients)
                {
                    var client = GetIrcClientByClientInfo(clientInfo);
                    this._sessionSupervisor.Connect(client);
                }
            else
                this._sessionSupervisor.Connect();
        }

        public void Disconnect(ObservableCollection<ClientInfo> clients)
        {
            if (clients != null)
                foreach (var clientInfo in clients)
                {
                    var client = GetClientByClientInfo(clientInfo);
                    this._sessionSupervisor.Disconnect(client);
                }
            else
                this._sessionSupervisor.Disconnect();
        }

        public void Reconnect(ObservableCollection<ClientInfo> clients, bool force = false)
        {
            if (clients != null)
                foreach (var clientInfo in clients)
                {
                    var client = GetClientByClientInfo(clientInfo);
                    this._sessionSupervisor.Reconnect(client, force);
                }
            else
                this._sessionSupervisor.Reconnect(force);
        }

        // *DetailsView
        public void SendMessage(ChannelItem message)
        {
            this._channelSupervisor.SendMessage(message);
        }

        public void SendMessage(MentionItem message)
        {
            this._channelSupervisor.SendMessage(message);
        }

        public void SendMessage(MessageItem message)
        {
            this._messageSupervisor.SendMessage(message);
        }

        public void SetNickName(IMessage target, string nickName)
        {
            this._sessionSupervisor.SetNickName(target, nickName);
        }

        public void SendAction(IMessage target, string message)
        {
            this._sessionSupervisor.SendAction(target, message);
        }

        public void SetTopic(IMessage target, string topic)
        {
            this._channelSupervisor.SetTopic(target, topic);
        }

        public void SetModes(IMessage target, string modes)
        {
            this._channelSupervisor.SetModes(target, modes);
        }

        #endregion

        #region Events

        void OnClientStatusChanged(object sender, ClientStatusEventArgs eventArgs)
        {
            var handler = this.ClientStatusChanged;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        void OnNetworkStatusChanged(object sender, NetworkStatusEventArgs eventArgs)
        {
            var handler = this.NetworkStatusChanged;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        void OnChannelSupervisor_ChannelJoined(object sender, ChannelStatusEventArgs e)
        {
            var handler = this.ChannelJoined;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void OnChannelSupervisor_ChannelLeft(object sender, ChannelStatusEventArgs e)
        {
            var handler = this.ChannelLeft;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void OnChannelSupervisor_MessageReceived(object sender, MessageItemEventArgs e)
        {
            var handler = this.ChannelItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void OnChannelSupervisor_MentionReceived(object sender, MessageItemEventArgs e)
        {
            var handler = this.MentionItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        void OnMessageSupervisor_MessageReceived(object sender, MessageItemEventArgs e)
        {
            var handler = this.MessageItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, e);
            }
        }

        #endregion

        #region Lookup methods

        public IrcLocalUser GetLocalUserBySummary(IMessage summary)
        {
            var result = (from client in this._clients
                          where client.Info.Id == summary.NetworkId
                          select client.Client.LocalUser).FirstOrDefault();
            return result;
        }

        public IrcChannel GetIrcChannelBySummary(IMessage summary)
        {
            var result = (from client in this._clients
                          from channel in client.Client.Channels
                          where client.Info.Id == summary.NetworkId
                          where channel.Name == summary.Name
                          select channel).FirstOrDefault();
            return result;
        }

        public IrcClient GetIrcClientBySummary(IMessage summary)
        {
            var result = (from client in this._clients
                          where client.Info.Id == summary.NetworkId
                          select client.Client).FirstOrDefault();
            return result;
        }

        public ClientItem GetClientByClientInfo(ClientInfo summary)
        {
            var result = (from client in this._clients
                          where client.Info.Id == summary.Id
                          select client).FirstOrDefault();
            return result;
        }

        public IrcClient GetIrcClientByClientInfo(ClientInfo summary)
        {
            var result = (from client in this._clients
                          where client.Info.Id == summary.Id
                          select client.Client).FirstOrDefault();
            return result;
        }

        public CtcpClient GetCtcpClientBySummary(IMessage summary)
        {
            var result = (from client in this._clients
                          where client.Info.Id == summary.NetworkId
                          select client.CtcpClient).FirstOrDefault();
            return result;
        }

        public Network GetNetworkByClient(IrcClient client)
        {
            return _sessionSupervisor.GetNetworkByClient(client);
        }

        public ClientItem GetClientByIrcClient(IrcClient client)
        {
            var result = (from clientItem in this._clients
                          where clientItem.Client == client
                          select clientItem).FirstOrDefault();
            return result;
        }

        #endregion

        private void InitializeSupervisors()
        {
            if (this._channelSupervisor == null)
            {
                this._channelSupervisor = new ChannelsSupervisor(_unitOfWork);
                this._channelSupervisor.ChannelJoined += new EventHandler<ChannelStatusEventArgs>(OnChannelSupervisor_ChannelJoined);
                this._channelSupervisor.ChannelLeft += new EventHandler<ChannelStatusEventArgs>(OnChannelSupervisor_ChannelLeft);
                this._channelSupervisor.ChannelItemReceived += new EventHandler<MessageItemEventArgs>(OnChannelSupervisor_MessageReceived);
                this._channelSupervisor.MentionItemReceived += new EventHandler<MessageItemEventArgs>(OnChannelSupervisor_MentionReceived);
            }

            if (this._messageSupervisor == null)
            {
                this._messageSupervisor = new MessagesSupervisor(_unitOfWork);
                this._messageSupervisor.MessageItemReceived += new EventHandler<MessageItemEventArgs>(OnMessageSupervisor_MessageReceived);
            }
        }

        public void UpdateStatus(IrcClient client, ClientState state, Exception error)
        {
            // TODO: SmartMonitor
            var foundClient = GetClientByIrcClient(client);
            foundClient.Info.State = state;
            foundClient.Info.Error = error;

            switch (foundClient.Info.State)
            {
                case ClientState.Inconceivable:
                    break;
                case ClientState.Connected:
                    break;
                case ClientState.Registered:
                    this.AttachLocalUser(client.LocalUser);
                    break;
                case ClientState.Processed:
                    break;
                case ClientState.Disconnected:
                    this.DetachLocalUser(client.LocalUser);
                    break;
                case ClientState.Error:
                    break;
                case ClientState.Intervention:
                    break;
                default:
                    break;
            }

            var eventArgs = new ClientStatusEventArgs()
            {
                Info = foundClient.Info,
            };
            this.OnClientStatusChanged(this, eventArgs);
        }

        public void UpdateClient(ClientItem client, IrcClient newClient)
        {
            var foundClient = this.Clients.Where(x => x.Info.Id == client.Info.Id).FirstOrDefault();
            if (foundClient != null)
            {
                foundClient.Client.Dispose();
                foundClient.Client = null;
                foundClient.Client = newClient;
            }
        }

        private void AttachLocalUser(IrcLocalUser localUser)
        {
            this.InitializeSupervisors();
            this._channelSupervisor.AttachLocalUser(localUser);
            this._messageSupervisor.AttachLocalUser(localUser);
        }

        private void DetachLocalUser(IrcLocalUser localUser)
        {
            if (this._channelSupervisor != null)
                this._channelSupervisor.DetachLocalUser(localUser);
            if (this._messageSupervisor != null)
                this._messageSupervisor.DetachLocalUser(localUser);
        }

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
                    if (this._statusObserver == null)
                        return;

                    this._statusObserver.Dispose();
                    this._statusObserver = null;
                    this._networkMonitor = null;
                    this.Shutdown();
                }
            }
            this._isDisposed = true;
        }
    }
}
