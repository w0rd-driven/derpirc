using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data
{
    [Table]
    public class ChannelMessage: BaseNotify, IBaseModel, IMessage
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<ChannelSummary> _details;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        // 1:M with IMessageSummary
        [Column(CanBeNull = false)]
        public int SummaryId { get; set; }
        [Association(Name = "Summary_Item", ThisKey = "SummaryId", OtherKey = "Id", IsForeignKey = true)]
        public ChannelSummary Summary
        {
            get { return _details.Entity; }
            set
            {
                ChannelSummary previousValue = _details.Entity;
                if (previousValue != value || _details.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _details.Entity = null;
                    }
                    _details.Entity = value;
                    if ((value != null))
                    {
                        SummaryId = value.Id;
                    }
                    else
                    {
                        SummaryId = default(int);
                    }
                    this.RaisePropertyChanged(() => SummaryId);
                    this.RaisePropertyChanged(() => Summary);
                }
            }
        }
        [Column(CanBeNull = true)]
        public DateTime TimeStamp { get; set; }
        [Column(CanBeNull = true)]
        public bool IsRead { get; set; }
        [Column(CanBeNull = true)]
        public string Command { get; set; }
        [Column(CanBeNull = true)]
        public byte[] Parameters { get; set; }
        [Column(CanBeNull = true)]
        public string Prefix { get; set; }
        [Column(CanBeNull = true)]
        public string Source { get; set; }
        [Column(CanBeNull = true)]
        public string RawContent { get; set; }

        public ChannelMessage()
        {
            _details = default(EntityRef<ChannelSummary>);
        }
    }
}