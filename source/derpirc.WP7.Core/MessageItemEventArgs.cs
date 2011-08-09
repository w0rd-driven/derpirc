using System;
using derpirc.Data;

namespace derpirc.Core
{
    public class MessageItemEventArgs : EventArgs
    {
        public MessageSummary User { get; set; }
        public MessageItem Message { get; set; }
    }
}
