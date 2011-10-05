using System;
using System.Collections.ObjectModel;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;
using Microsoft.Phone.Reactive;

namespace derpirc.Core
{
    public class SupervisorFacade
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

        private DataUnitOfWork _unitOfWork;
        private SettingsUnitOfWork _unitOfWorkSettings;

        private NetworkMonitor _networkMonitor;
        private IDisposable statusObserver;

        private SessionSupervisor _sessionSupervisor;
        private ChannelsSupervisor _channelSupervisor;
        private MessagesSupervisor _messageSupervisor;

        // HACK: UI facing
        public event EventHandler<ChannelStatusEventArgs> ChannelJoined;
        public event EventHandler<ChannelStatusEventArgs> ChannelLeft;
        public event EventHandler<MessageItemEventArgs> ChannelItemReceived;
        public event EventHandler<MessageItemEventArgs> MentionItemReceived;
        public event EventHandler<MessageItemEventArgs> MessageItemReceived;

        public event EventHandler<NetworkStatusEventArgs> NetworkStatusChanged;
        public event EventHandler<ClientStatusEventArgs> ClientStatusChanged;

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

        public SupervisorFacade()
        {
            _clients = new ObservableCollection<ClientItem>();
            Startup();
        }

        private void Startup()
        {
            _networkMonitor = new NetworkMonitor(10000);
            statusObserver = _networkMonitor.Status()
                .ObserveOnDispatcher()
                .Subscribe(type =>
                {
                    NetworkType = type;
                    if (type != NetworkType.None)
                        IsNetworkAvailable = true;
                    else
                        IsNetworkAvailable = false;
                    var eventArgs = new NetworkStatusEventArgs()
                    {
                        IsAvailable = IsNetworkAvailable,
                        Type = type,
                    };
                    OnNetworkStatusChanged(this, eventArgs);
                });
            // HACK: Test First Init
            _unitOfWork = new DataUnitOfWork();
            _unitOfWorkSettings = new SettingsUnitOfWork();
        }

        private void Shutdown()
        {
            //if (this.statusObserver == null)
            //    return;

            //this.statusObserver.Dispose();
            //this.statusObserver = null;
            //_networkMonitor = null;
            _sessionSupervisor = null;
            _channelSupervisor = null;
            _messageSupervisor = null;
        }

        private void InitializeSupervisors()
        {
            if (_channelSupervisor == null)
            {
                _channelSupervisor = new ChannelsSupervisor(_unitOfWork);
                _channelSupervisor.ChannelJoined += new EventHandler<ChannelStatusEventArgs>(_channelSupervisor_ChannelJoined);
                _channelSupervisor.ChannelLeft += new EventHandler<ChannelStatusEventArgs>(_channelSupervisor_ChannelLeft);
                _channelSupervisor.ChannelItemReceived += new EventHandler<MessageItemEventArgs>(_channelSupervisor_MessageReceived);
                _channelSupervisor.MentionItemReceived += new EventHandler<MessageItemEventArgs>(_channelSupervisor_MentionReceived);
            }

            if (_messageSupervisor == null)
            {
                _messageSupervisor = new MessagesSupervisor(_unitOfWork);
                _messageSupervisor.MessageItemReceived += new EventHandler<MessageItemEventArgs>(_messageSupervisor_MessageReceived);
            }
        }

        #region Events

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

        void OnNetworkStatusChanged(object sender, NetworkStatusEventArgs eventArgs)
        {
            var handler = NetworkStatusChanged;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        void OnClientStatusChanged(object sender, ClientStatusEventArgs eventArgs)
        {
            var handler = ClientStatusChanged;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        #endregion

        #region Lookup Methods

        public IrcLocalUser GetLocalUserBySummary(IMessage channel)
        {
            var result = (from client in _clients
                          where client.Id == channel.NetworkId
                          select client.Client.LocalUser).FirstOrDefault();
            return result;
        }

        public Network GetNetworkByClient(IrcClient client)
        {
            return _sessionSupervisor.GetNetworkByClient(client);
        }

        public ClientItem GetClientByIrcClient(IrcClient client)
        {
            var result = (from clientItem in _clients
                          where clientItem.Client == client
                          select clientItem).FirstOrDefault();
            return result;
        }

        public void UpdateStatus(IrcClient client, ClientState state, Exception error)
        {
            var foundClient = GetClientByIrcClient(client);
            foundClient.State = state;
            foundClient.Error = error;

            switch (foundClient.State)
            {
                case ClientState.Inconceivable:
                    break;
                case ClientState.Connected:
                    break;
                case ClientState.Registered:
                    AttachLocalUser(client.LocalUser);
                    break;
                case ClientState.Processed:
                    break;
                case ClientState.Disconnected:
                    DetachLocalUser(client.LocalUser);
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
                Client = foundClient,
            };
            OnClientStatusChanged(this, eventArgs);
        }

        private void AttachLocalUser(IrcLocalUser localUser)
        {
            InitializeSupervisors();
            _channelSupervisor.AttachLocalUser(localUser);
            _messageSupervisor.AttachLocalUser(localUser);
        }

        private void DetachLocalUser(IrcLocalUser localUser)
        {
            _channelSupervisor.DetachLocalUser(localUser);
            _messageSupervisor.DetachLocalUser(localUser);
        }

        #endregion

        #region Passthrough Methods

        public void Initialize()
        {
            if (_sessionSupervisor == null)
                _sessionSupervisor = new SessionSupervisor(_unitOfWork, _unitOfWorkSettings);
            _sessionSupervisor.Initialize();
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

        #endregion

    }
}
