using System;
using System.Collections.Generic;

namespace derpirc.Data
{
    public interface IMessageItem
    {
        int SummaryId { get; set; }
        DateTime TimeStamp { get; set; }
        bool IsRead { get; set; }
        string Source { get; set; }
        string Text { get; set; }
        MessageType Type { get; set; }
    }
}
