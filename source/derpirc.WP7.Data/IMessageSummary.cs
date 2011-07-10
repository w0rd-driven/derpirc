using System;
using System.Collections.Generic;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    public interface IMessageSummary
    {
        // Foreign keys
        // 1:1 with IMessageDetail
        int DetailId { get; set; }
        int ServerId { get; set; }
        int LastItemId { get; set; }

        string Name { get; set; }
        int Count { get; set; }
        int UnreadCount { get; set; }
    }
}