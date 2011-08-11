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
        private EntityRef<Session> _session;
        private EntityRef<Server> _server;
        private EntityRef<SessionNetwork> _network;
        private EntitySet<ChannelSummary> _channels;
        private EntitySet<MentionSummary> _mentions;
        private EntitySet<MessageSummary> _messages;

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

        [Column(CanBeNull = false)]
        public int SessionId { get; set; }
        [Association(Name = "Session_Item", ThisKey = "SessionId", OtherKey = "Id", IsForeignKey = true)]
        public Session Session
        {
            get { return _session.Entity; }
            set
            {
                Session previousValue = _session.Entity;
                if ((previousValue != value || this._server.HasLoadedOrAssignedValue == false))
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
                if ((previousValue != value || this._server.HasLoadedOrAssignedValue == false))
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

        [Column(CanBeNull = true)]
        public int NetworkId { get; set; }
        [Association(Name = "Network_Item", ThisKey = "NetworkId", OtherKey = "Id", IsForeignKey = false)]
        public SessionNetwork Network
        {
            get { return _network.Entity; }
            set
            {
                SessionNetwork previousValue = _network.Entity;
                if ((previousValue != value || this._server.HasLoadedOrAssignedValue == false))
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
                    var newValue = value as FixupCollection<ChannelSummary>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupChannels;
                    }
                }
            }
        }

        [Association(Name = "Mention_Items", ThisKey = "Id", OtherKey = "ServerId", DeleteRule = "NO ACTION")]
        public ICollection<MentionSummary> Mentions
        {
            get
            {
                return _mentions;
            }
            set
            {
                if (!ReferenceEquals(_mentions, value))
                {
                    var previousValue = _mentions;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupMentions;
                    }
                    _mentions.SetSource(value);
                    var newValue = value as FixupCollection<MentionSummary>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupMentions;
                    }
                }
            }
        }

        [Association(Name = "Message_Items", ThisKey = "Id", OtherKey = "ServerId", DeleteRule = "NO ACTION")]
        public ICollection<MessageSummary> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                if (!ReferenceEquals(_messages, value))
                {
                    var previousValue = _messages;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupMessages;
                    }
                    _messages.SetSource(value);
                    var newValue = value as FixupCollection<MessageSummary>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupMessages;
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

        private void FixupMentions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MentionSummary item in e.NewItems)
                    item.Server = this;
            }
            if (e.OldItems != null)
            {
                foreach (MentionSummary item in e.OldItems)
                {
                    if (ReferenceEquals(item.Server, this))
                        item.Server = null;
                }
            }
        }

        private void FixupMessages(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MessageSummary item in e.NewItems)
                    item.Server = this;
            }
            if (e.OldItems != null)
            {
                foreach (MessageSummary item in e.OldItems)
                {
                    if (ReferenceEquals(item.Server, this))
                        item.Server = null;
                }
            }
        }

        #endregion

        public SessionServer()
        {
            _session = default(EntityRef<Session>);
            _server = default(EntityRef<Server>);
            _network = default(EntityRef<SessionNetwork>);
            _channels = new EntitySet<ChannelSummary>();
            _channels.CollectionChanged += FixupChannels;
            _mentions = new EntitySet<MentionSummary>();
            _mentions.CollectionChanged += FixupMentions;
            _messages = new EntitySet<MessageSummary>();
            _messages.CollectionChanged += FixupMessages;
        }
    }
}
