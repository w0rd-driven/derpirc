using System;
using derpirc.Data;

namespace derpirc.Core
{
    public class ChannelMessageEventArgs : EventArgs
    {
        public ChannelSummary Channel { get; set; }
        public ChannelMessage Message { get; set; }
    }
}
