using System.ComponentModel;

namespace derpirc.Data
{
    /// <summary>
    /// List-based Item
    /// </summary>
    public class MentionsView : BaseModel<MentionsView>, IMessagesView
    {
        // IMessagesView
        public int ServerId { get; set; }
        public string Name { get; set; }
        public IMessage LastItem { get; set; }
        public int Count { get; set; }
        public int UnreadCount { get; set; }
    }
}
