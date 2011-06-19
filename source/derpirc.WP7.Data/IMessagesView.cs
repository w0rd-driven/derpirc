using System;
using System.Collections.Generic;

namespace derpirc.Data
{
    public interface IMessagesView
    {
        // Foreign keys
        int ServerId { get; set; }

        string Name { get; set; }
        IMessage LastItem { get; set; }
        int Count { get; set; }
        int UnreadCount { get; set; }
    }
}