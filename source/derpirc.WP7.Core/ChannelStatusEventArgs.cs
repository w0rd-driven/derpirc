using System;

namespace derpirc.Core
{
    public class ChannelStatusEventArgs : EventArgs
    {
        public int SummaryId { get; set; }
        public string NetworkName { get; set; }
        public string ChannelName { get; set; }
        public ChannelStatus Status { get; set; }
    }
}
