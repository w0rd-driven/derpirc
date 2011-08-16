using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models
{
    [Table]
    public partial class Network : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Session> _session;
        private EntityRef<Server> _server;
        private EntitySet<ChannelSummary> _channels;
        private EntitySet<MentionSummary> _mentions;
        private EntitySet<MessageSummary> _messages;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }
        [Column(CanBeNull = true)]
        public bool IsJoinEnabled { get; set; }
        [Column(CanBeNull = true)]
        public string JoinChannels { get; set; }

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

        [Column(CanBeNull = false)]
        public int ServerId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "ServerId", OtherKey = "Id", IsForeignKey = true)]
        public Server Server
        {
            get { return _server.Entity; }
            set
            {
                Server previousValue = _server.Entity;
                if (previousValue != value || _server.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _server.Entity = null;
                    }
                    _server.Entity = value;
                    if ((value != null))
                    {
                        ServerId = value.Id;
                    }
                    else
                    {
                        ServerId = default(int);
                    }
                    this.RaisePropertyChanged(() => ServerId);
                    this.RaisePropertyChanged(() => Server);
                }
            }
        }

        [Association(Name = "Channel_Items", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
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

        [Association(Name = "Mention_Items", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
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

        [Association(Name = "Message_Items", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
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
                    item.Network = this;
            }
            if (e.OldItems != null)
            {
                foreach (ChannelSummary item in e.OldItems)
                {
                    if (ReferenceEquals(item.Network, this))
                        item.Network = null;
                }
            }
        }

        private void FixupMentions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MentionSummary item in e.NewItems)
                    item.Network = this;
            }
            if (e.OldItems != null)
            {
                foreach (MentionSummary item in e.OldItems)
                {
                    if (ReferenceEquals(item.Network, this))
                        item.Network = null;
                }
            }
        }

        private void FixupMessages(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MessageSummary item in e.NewItems)
                    item.Network = this;
            }
            if (e.OldItems != null)
            {
                foreach (MessageSummary item in e.OldItems)
                {
                    if (ReferenceEquals(item.Network, this))
                        item.Network = null;
                }
            }
        }

        #endregion

        public Network()
        {
            _session = default(EntityRef<Session>);
            _server = default(EntityRef<Server>);
            _channels = new EntitySet<ChannelSummary>();
            _channels.CollectionChanged += FixupChannels;
            _mentions = new EntitySet<MentionSummary>();
            _mentions.CollectionChanged += FixupMentions;
            _messages = new EntitySet<MessageSummary>();
            _messages.CollectionChanged += FixupMessages;
        }
    }
}
