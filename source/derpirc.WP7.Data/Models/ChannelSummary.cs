using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using derpirc.Data.Models.Settings;

namespace derpirc.Data.Models
{
    [Table]
    public class ChannelSummary : BaseNotify, IBaseModel, IMessageSummary
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Network> _network;
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
        public int NetworkId { get; set; }
        [Association(Name = "Network_Item", ThisKey = "NetworkId", OtherKey = "Id", IsForeignKey = true)]
        public Network Network
        {
            get
            {
                return this._network.Entity;
            }
            set
            {
                Network previousValue = this._network.Entity;
                if ((previousValue != value) || (this._network.HasLoadedOrAssignedValue == false))
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        this._network.Entity = null;
                        //previousValue.Channels.Remove(this);
                    }
                    this._network.Entity = value;
                    if ((value != null))
                    {
                        //value.Channels.Add(this);
                        this.NetworkId = value.Id;
                    }
                    else
                    {
                        this.NetworkId = default(int);
                    }
                    this.RaisePropertyChanged(() => NetworkId);
                    this.RaisePropertyChanged(() => Network);
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
            _network = default(EntityRef<Network>);
            _messages = new EntitySet<ChannelItem>();
            _messages.CollectionChanged += FixupMessages;
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
    }
}
