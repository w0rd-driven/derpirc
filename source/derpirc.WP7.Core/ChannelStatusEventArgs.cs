using System;
using derpirc.Data.Models;

namespace derpirc.Core
{
    public class ChannelStatusEventArgs : EventArgs
    {
        public int SummaryId { get; set; }
        public ChannelStatusType Status { get; set; }
    }
}
