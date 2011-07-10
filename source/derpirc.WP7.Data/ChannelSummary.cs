using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    /// <summary>
    /// List-based Item
    /// </summary>
    [Table]
    public class ChannelSummary : BaseNotify, IBaseModel, IMessageSummary
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Server> _server;
        private EntitySet<ChannelMessage> _messages;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public int ServerId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "ServerId", OtherKey = "Id", IsForeignKey = false)]
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
                        this.ServerId = value.Id;
                    }
                    else
                    {
                        this.ServerId = default(int);
                    }
                    this.RaisePropertyChanged(() => ServerId);
                    this.RaisePropertyChanged(() => Server);
                }
            }
        }
        [Column(CanBeNull = false)]
        public string Name { get; set; }
        [Association(Name = "Message_Items", ThisKey = "Id", OtherKey = "SummaryId", DeleteRule = "NO ACTION")]
        public EntitySet<ChannelMessage> Messages
        {
            get { return _messages; }
            set { _messages.Assign(value); }
        }

        [Column(CanBeNull = true)]
        public int LastItemId { get; set; }
        public IMessage LastItem { get; set; }

        [Column(CanBeNull = true)]
        public int Count { get; set; }
        [Column(CanBeNull = true)]
        public int UnreadCount { get; set; }
        [Column(CanBeNull = true)]
        public string Topic { get; set; }

        public ChannelSummary()
        {
            _server = default(EntityRef<Server>);
            _messages = new EntitySet<ChannelMessage>(new Action<ChannelMessage>(attach_Messages), new Action<ChannelMessage>(detach_Messages));
            //TODO: Link up Detail collection events to get LastItemId, LastItem, Count, and UnreadCount
        }

        void attach_Messages(ChannelMessage entity)
        {
            this.RaisePropertyChanged();
            entity.Summary = this;
        }

        void detach_Messages(ChannelMessage entity)
        {
            this.RaisePropertyChanged();
            entity.Summary = null;
        }
    }
}