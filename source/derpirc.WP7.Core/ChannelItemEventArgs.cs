using System;
using derpirc.Data;

namespace derpirc.Core
{
    public class ChannelItemEventArgs : EventArgs
    {
        public ChannelSummary Channel { get; set; }
        public ChannelItem Message { get; set; }
    }
}
