﻿using System.Data.Linq;

namespace derpirc.Data.Models
{
    public interface IMessageSummary
    {
        int NetworkId { get; set; }
        Network Network { get; set; }
        string Name { get; set; }
        int LastItemId { get; set; }
        IMessageItem LastItem { get; set; }
        int Count { get; set; }
        int UnreadCount { get; set; }
        // Not used because we segregate into Channel/Mention/Message tables
        //EntitySet<IMessageItem> Messages { get; set; }
    }
}
