using System;

namespace derpirc.Data.Models
{
    public interface IMessage
    {
        int NetworkId { get; set; }
        Network Network { get; set; }
        string Name { get; set; }
        Nullable<int> LastItemId { get; set; }
        IMessageItem LastItem { get; set; }
        Nullable<int> Count { get; set; }
        Nullable<int> UnreadCount { get; set; }
        // Not used because we segregate into Summary/Summary/Summary tables
        //EntitySet<IMessageItem> Messages { get; set; }
    }
}
