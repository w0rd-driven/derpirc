using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data
{
    [Table]
    public class ChannelItem : BaseNotify, IBaseModel, IMessageItem
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<ChannelSummary> _summary;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = true)]
        public DateTime TimeStamp { get; set; }
        [Column(CanBeNull = true)]
        public bool IsRead { get; set; }
        [Column(CanBeNull = true)]
        public string Source { get; set; }
        [Column(CanBeNull = true)]
        public string Text { get; set; }
        [Column(CanBeNull = true)]
        public MessageType Type { get; set; }

        #endregion

        #region Navigation Properties

        [Column(CanBeNull = false)]
        public int SummaryId { get; set; }
        [Association(Name = "Summary_Item", ThisKey = "SummaryId", OtherKey = "Id", IsForeignKey = true)]
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

        #endregion

        public ChannelItem()
        {
            _summary = default(EntityRef<ChannelSummary>);
        }
    }
}
