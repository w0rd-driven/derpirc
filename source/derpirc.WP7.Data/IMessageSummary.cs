using System;
using System.Collections.Generic;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    public interface IMessageSummary
    {
        int ServerId { get; set; }
        SessionServer Server { get; set; }
        string Name { get; set; }
        int LastItemId { get; set; }
        IMessageItem LastItem { get; set; }
        int Count { get; set; }
        int UnreadCount { get; set; }
        // Not used because we segregate into Channel/Mention/Message tables
        //ICollection<IMessageItem> Messages { get; set; }
    }
}
