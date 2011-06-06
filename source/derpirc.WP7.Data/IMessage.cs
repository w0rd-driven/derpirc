using System;
using System.Collections.Generic;

namespace derpirc.Data
{
    public interface IMessage
    {
        // Foreign keys
        // 1:1 with IMessagesView
        int ListId { get; set; }

        DateTime TimeStamp { get; set; }
    }
}