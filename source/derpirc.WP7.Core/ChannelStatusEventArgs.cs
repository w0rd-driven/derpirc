using System;

namespace derpirc.Core
{
    public class ChannelStatusEventArgs : EventArgs
    {
        public int SummaryId { get; set; }
        public ChannelStatus Status { get; set; }
    }
}
