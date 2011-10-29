using System;

namespace derpirc.Data.Models
{
    public interface IMessageItem
    {
        int Id { get; set; }
        int SummaryId { get; set; }
        DateTime Timestamp { get; set; }
        bool IsRead { get; set; }
        string Source { get; set; }
        string Text { get; set; }
        Owner Owner { get; set; }
    }
}
