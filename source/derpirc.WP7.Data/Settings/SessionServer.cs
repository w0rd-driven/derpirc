using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table(Name = "SessionServers")]
    public partial class SessionServer : BaseNotify, IBaseModel, ISessionServer, IServer
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Server> _server;
        private EntityRef<Session> _session;
        private EntityRef<SessionNetwork> _network;
        private EntitySet<ChannelSummary> _channels;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = true)]
        public string DisplayName { get; set; }
        [Column(CanBeNull = false)]
        public string HostName { get; set; }
        [Column(CanBeNull = true)]
        public int Port { get; set; }
        [Column(CanBeNull = true)]
        public string Ports { get; set; }
        [Column(CanBeNull = true)]
        public string Group { get; set; }
        [Column(CanBeNull = true)]
        public string Password { get; set; }

        #endregion

        #region Navigation Properties

        [Column(CanBeNull = true)]
        public int BasedOnId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "BasedOnId", OtherKey = "Id", IsForeignKey = true)]
        public Server Server
        {
            get
            {
                return this._server.Entity;
            }
            set
            {
                Server previousValue = this._server.Entity;
                if (previousValue != value || this._server.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        this._server.Entity = null;
                    }
                    this._server.Entity = value;
                    if ((value != null))
                    {
                        this.BasedOnId = value.Id;
                    }
                    else
                    {
                        this.BasedOnId = default(int);
                    }
                    this.RaisePropertyChanged(() => BasedOnId);
                    this.RaisePropertyChanged(() => Server);
                }
            }
        }

        [Column(CanBeNull = false)]
        public int SessionId { get; set; }
        [Association(Name = "Session_Item", ThisKey = "SessionId", OtherKey = "Id", IsForeignKey = true)]
        public Session Session
        {
            get { return _session.Entity; }
            set
            {
                Session previousValue = _session.Entity;
                if (previousValue != value || _session.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _session.Entity = null;
                    }
                    _session.Entity = value;
                    if ((value != null))
                    {
                        SessionId = value.Id;
                    }
                    else
                    {
                        SessionId = default(int);
                    }
                    this.RaisePropertyChanged(() => SessionId);
                    this.RaisePropertyChanged(() => Session);
                }
            }
        }

        [Column(CanBeNull = true)]
        public int NetworkId { get; set; }
        [Association(Name = "Network_Item", ThisKey = "NetworkId", OtherKey = "Id", IsForeignKey = false)]
        public SessionNetwork Network
        {
            get { return _network.Entity; }
            set
            {
                SessionNetwork previousValue = _network.Entity;
                if (previousValue != value || _network.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _network.Entity = null;
                    }
                    _network.Entity = value;
                    if ((value != null))
                    {
                        NetworkId = value.Id;
                    }
                    else
                    {
                        NetworkId = default(int);
                    }
                    this.RaisePropertyChanged(() => NetworkId);
                    this.RaisePropertyChanged(() => Network);
                }
            }
        }

        [Association(Name = "Channel_Items", ThisKey = "Id", OtherKey = "ServerId", DeleteRule = "NO ACTION")]
        public ICollection<ChannelSummary> Channels
        {
            get
            {
                return _channels;
            }
            set
            {
                if (!ReferenceEquals(_channels, value))
                {
                    var previousValue = _channels;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupChannels;
                    }
                    _channels.SetSource(value);
                    //_messages = value;
                    var newValue = value as FixupCollection<ChannelSummary>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupChannels;
                    }
                }
            }
        }

        #endregion

        #region Association Fixup

        private void FixupChannels(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ChannelSummary item in e.NewItems)
                    item.Server = this;
            }
            if (e.OldItems != null)
            {
                foreach (ChannelSummary item in e.OldItems)
                {
                    if (ReferenceEquals(item.Server, this))
                        item.Server = null;
                }
            }
        }

        #endregion

        public SessionServer()
        {
            _server = default(EntityRef<Server>);
            _session = default(EntityRef<Session>);
            _network = default(EntityRef<SessionNetwork>);
            _channels = new EntitySet<ChannelSummary>();
            _channels.CollectionChanged += FixupChannels;
            //_channels = new EntitySet<ChannelSummary>(new Action<ChannelSummary>(attach_Channels), new Action<ChannelSummary>(detach_Channels));
        }

        private void attach_Channels(ChannelSummary entity)
        {
            //this.RaisePropertyChanged();
            entity.Server = this;
        }

        private void detach_Channels(ChannelSummary entity)
        {
            //this.RaisePropertyChanged();
            entity.Server = null;
        }
    }
}
