using System;
using System.Collections.Generic;

namespace derpirc.Data
{
    public interface IMessageView
    {
        // Foreign keys
        // 1:1 with IMessageSummary
        int ListId { get; set; }

        IList<IMessage> Items { get; set; }
    }
}