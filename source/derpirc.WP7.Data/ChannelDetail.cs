using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data
{
    [Table]
    public class ChannelDetail : BaseNotify, IBaseModel, IMessageDetail
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<ChannelSummary> _summary;
        private EntitySet<ChannelMessage> _messages;

        // 1:1 with IMessageSummary
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        [Association(Name = "Summary_Item", ThisKey = "Id", OtherKey = "Id", IsForeignKey = true)]
        public ChannelSummary Summary
        {
            get { return _summary.Entity; }
            set
            {
                ChannelSummary previousValue = _summary.Entity;
                if (previousValue != value || _summary.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _summary.Entity = null;
                    }
                    _summary.Entity = value;
                    if ((value != null))
                    {
                        Id = value.DetailId;
                    }
                    else
                    {
                        Id = default(int);
                    }
                    this.RaisePropertyChanged(() => Id);
                    this.RaisePropertyChanged(() => Summary);
                }
            }
        }

        [Association(Name = "Message_Items", ThisKey = "Id", OtherKey = "Id", DeleteRule = "NO ACTION")]
        public EntitySet<ChannelMessage> Messages
        {
            get { return _messages; }
            set { _messages.Assign(value); } 
        }

        public ChannelDetail()
        {
            _summary = default(EntityRef<ChannelSummary>);
            _messages = new EntitySet<ChannelMessage>(new Action<ChannelMessage>(attach_Messages), new Action<ChannelMessage>(detach_Messages));
        }

        void attach_Messages(ChannelMessage entity)
        {
            this.RaisePropertyChanged();
            entity.Details = this;
        }

        void detach_Messages(ChannelMessage entity)
        {
            this.RaisePropertyChanged();
            entity.Details = null;
        }
    }
}