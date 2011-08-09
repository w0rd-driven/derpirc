using System;
using System.Collections.Generic;

namespace derpirc.Data
{
    public interface IMessageItem
    {
        // Foreign keys
        // 1:M from IMessageSummary
        int SummaryId { get; set; }
        DateTime TimeStamp { get; set; }
        bool IsRead { get; set; }
        // IrcDotNet.IIrcMessageSource
        string Source { get; set; }
        string Text { get; set; }
        MessageType Type { get; set; }
    }
}
