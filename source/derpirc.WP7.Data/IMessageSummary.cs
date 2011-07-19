using System;
using System.Collections.Generic;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    public interface IMessageSummary
    {
        string Name { get; set; }

        int LastItemId { get; set; }
        IMessage LastItem { get; set; }

        int Count { get; set; }
        int UnreadCount { get; set; }

        int ServerId { get; set; }
        Server Server { get; set; }
        // Not used because we segregate into Channel/Mention/Message tables
        //ICollection<IMessage> Messages { get; set; }
    }
}