using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data
{
    [Table]
    public class ChannelMessage: BaseNotify, IBaseModel, IMessage
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<ChannelDetail> _details;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        // 1:1 with IMessageDetail
        [Column(CanBeNull = false)]
        public int DetailId { get; set; }
        [Association(Name = "Detail_Item", ThisKey = "DetailId", OtherKey = "Id", IsForeignKey = true)]
        public ChannelDetail Details
        {
            get { return _details.Entity; }
            set
            {
                ChannelDetail previousValue = _details.Entity;
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
                        DetailId = value.Id;
                    }
                    else
                    {
                        DetailId = default(int);
                    }
                    this.RaisePropertyChanged(() => DetailId);
                    this.RaisePropertyChanged(() => Details);
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

        }
    }
}