﻿using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data
{
    [Table]
    public class ChannelMessage: BaseNotify, IBaseModel, IMessage
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<ChannelSummary> _summary;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        // 1:M with IMessageSummary
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
        [Column(CanBeNull = true)]
        public DateTime TimeStamp { get; set; }
        [Column(CanBeNull = true)]
        public bool IsRead { get; set; }
        [Column(CanBeNull = true)]
        public string Source { get; set; }
        [Column(CanBeNull = true)]
        public string Text { get; set; }

        public ChannelMessage()
        {
            _summary = default(EntityRef<ChannelSummary>);
        }
    }
}
