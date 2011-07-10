using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data
{
    [Table]
    public class ChannelMessage: BaseNotify, IBaseModel, IMessage
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public int ListId { get; set; }
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

        [Column(IsVersion = true)]
        private Binary version;

        public ChannelMessage()
        {

        }
    }
}
