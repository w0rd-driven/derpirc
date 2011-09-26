using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace derpirc.Data.Models
{
    [Table]
    public class Message : BaseNotify, IBaseModel, IMessage
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Network> _network;
        private EntitySet<MessageItem> _messages;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string Name { get; set; }

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
        public EntitySet<MessageItem> Messages
        {
            get { return _messages; }
            set { _messages.Assign(value); }
        }

        #endregion

        public Message()
        {
            _network = default(EntityRef<Network>);
            _messages = new EntitySet<MessageItem>(new Action<MessageItem>(attach_Messages), new Action<MessageItem>(detach_Messages));
        }

        void attach_Messages(MessageItem entity)
        {
            //this.RaisePropertyChanged();
            UpdateMessageCounts(entity);
            entity.Summary = this;
        }

        void detach_Messages(MessageItem entity)
        {
            //this.RaisePropertyChanged();
            UpdateMessageCounts(entity);
            entity.Summary = null;
        }

        private void UpdateMessageCounts(MessageItem entity)
        {
            LastItem = entity;
            // Id is 0 here so inflate counts blindly
            LastItemId = entity.Id;
            Count = _messages.Count + 1;
            UnreadCount = _messages.Count(x => x.IsRead == false) + 1;
        }
    }
}
