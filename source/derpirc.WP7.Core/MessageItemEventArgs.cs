using System;

namespace derpirc.Core
{
    /// <summary>
    /// Generic EventArgs for lookup. Instead of passing entire records, just pass what is needed to lookup in cache
    /// </summary>
    public class MessageItemEventArgs : EventArgs
    {
        public int NetworkId { get; set; }
        public int SummaryId { get; set; }
        public int MessageId { get; set; }
    }
}
