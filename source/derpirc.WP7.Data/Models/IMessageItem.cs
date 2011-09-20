using System;

namespace derpirc.Data.Models
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
