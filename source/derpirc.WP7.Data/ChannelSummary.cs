using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    [Table]
    public class ChannelSummary : BaseNotify, IBaseModel, IMessageSummary
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<SessionServer> _server;
        private EntitySet<ChannelItem> _messages;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string Name { get; set; }
        [Column(CanBeNull = true)]
        public string Topic { get; set; }

        [Column(CanBeNull = true)]
        public int LastItemId { get; set; }
        public IMessageItem LastItem { get; set; }

        [Column(CanBeNull = true)]
        public int Count { get; set; }
        [Column(CanBeNull = true)]
        public int UnreadCount { get; set; }

        #endregion

        #region Navigation Properties

        [Column(CanBeNull = false)]
        public int ServerId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "ServerId", OtherKey = "Id", IsForeignKey = false)]
        public SessionServer Server
        {
            get
            {
                return this._server.Entity;
            }
            set
            {
                SessionServer previousValue = this._server.Entity;
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

        [Association(Name = "Message_Items", ThisKey = "Id", OtherKey = "SummaryId", DeleteRule = "NO ACTION")]
        public ICollection<ChannelItem> Messages
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
                    //_messages = value;
                    var newValue = value as FixupCollection<ChannelItem>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupMessages;
                    }
                }
            }
        }

        #endregion

        #region Association Fixup
    
        private void FixupMessages(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ChannelItem item in e.NewItems)
                    item.Summary = this;
                // HACK: Not locking can cause weird behaviors during commits. 
                UpdateMessageCounts();
            }
            if (e.OldItems != null)
            {
                foreach (ChannelItem item in e.OldItems)
                {
                    if (ReferenceEquals(item.Summary, this))
                        item.Summary = null;
                }
            }
        }

        #endregion

        public ChannelSummary()
        {
            _server = default(EntityRef<SessionServer>);
            _messages = new EntitySet<ChannelItem>();
            _messages.CollectionChanged += FixupMessages;
            //_messages = new EntitySet<ChannelMessage>(new Action<ChannelMessage>(attach_Messages), new Action<ChannelMessage>(detach_Messages));
        }

        private void UpdateMessageCounts()
        {
            var lastIndex = _messages.Count - 1;
            var lastItem = _messages[lastIndex] as ChannelItem;
            LastItem = lastItem;
            LastItemId = lastItem.Id;
            Count = _messages.Count;
            UnreadCount = _messages.Count(x => x.IsRead == false);
        }

        //void attach_Messages(ChannelMessage entity)
        //{
        ////    this.RaisePropertyChanged();
        //    UpdateMessageCounts();
        //    entity.Summary = this;
        //}

        //void detach_Messages(ChannelMessage entity)
        //{
        ////    this.RaisePropertyChanged();
        //    UpdateMessageCounts();
        //    entity.Summary = null;
        //}
    }
}
