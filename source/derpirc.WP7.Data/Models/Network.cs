using System;
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
        private EntitySet<Channel> _channels;
        private EntitySet<Mention> _mentions;
        private EntitySet<Message> _messages;

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
        public EntitySet<Channel> Channels
        {
            get { return _channels; }
            set { _channels.Assign(value); }
        }

        [Association(Name = "Mention_Items", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
        public EntitySet<Mention> Mentions
        {
            get { return _mentions; }
            set { _mentions.Assign(value); }
        }

        [Association(Name = "Message_Items", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
        public EntitySet<Message> Messages
        {
            get { return _messages; }
            set { _messages.Assign(value); }
        }

        #endregion

        public Network()
        {
            _session = default(EntityRef<Session>);
            _server = default(EntityRef<Server>);
            _channels = new EntitySet<Channel>(new Action<Channel>(attach_Channels), new Action<Channel>(detach_Channels));
            _mentions = new EntitySet<Mention>(new Action<Mention>(attach_Mentions), new Action<Mention>(detach_Mentions));
            _messages = new EntitySet<Message>(new Action<Message>(attach_Messages), new Action<Message>(detach_Messages));
        }

        void attach_Channels(Channel entity)
        {
            //this.RaisePropertyChanged();
            entity.Network = this;
        }

        void detach_Channels(Channel entity)
        {
            //this.RaisePropertyChanged();
            entity.Network = null;
        }

        void attach_Mentions(Mention entity)
        {
            //this.RaisePropertyChanged();
            entity.Network = this;
        }

        void detach_Mentions(Mention entity)
        {
            //this.RaisePropertyChanged();
            entity.Network = null;
        }

        void attach_Messages(Message entity)
        {
            //this.RaisePropertyChanged();
            entity.Network = this;
        }

        void detach_Messages(Message entity)
        {
            //this.RaisePropertyChanged();
            entity.Network = null;
        }
    }
}
