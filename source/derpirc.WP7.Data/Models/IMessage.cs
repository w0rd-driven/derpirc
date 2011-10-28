using System;

namespace derpirc.Data.Models
{
    public interface IMessage
    {
        int Id { get; set; }
        int NetworkId { get; set; }
        Network Network { get; set; }
        string Name { get; set; }
        // Not used because we segregate into Channel/Mention/Message tables
        //EntitySet<IMessageItem> Messages { get; set; }
    }
}
