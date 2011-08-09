using System;
using derpirc.Data;

namespace derpirc.Core
{
    public class ChannelStatusEventArgs : EventArgs
    {
        public ChannelSummary Channel { get; set; }
        public ChannelStatusTypeEnum Status { get; set; }
    }
}
