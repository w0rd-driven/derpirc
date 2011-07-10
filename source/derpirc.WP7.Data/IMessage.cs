using System;
using System.Collections.Generic;

namespace derpirc.Data
{
    public interface IMessage
    {
        // Foreign keys
        // 1:M from IMessageDetail
        int SummaryId { get; set; }

        DateTime TimeStamp { get; set; }
        bool IsRead { get; set; }

        // IrcDotNet.IrcRawMessageEventArgs.Message
        // IrcDotNet.IrcClient.IrcMessage
        string Command { get; set; }
        // Converted to byte[]/Binary from string[] to reduce the need for a child table
        byte[] Parameters { get; set; }
        string Prefix { get; set; }
        // IrcDotNet.IIrcMessageSource
        string Source { get; set; }

        // IrcDotNet.IrcRawMessageEventArgs.RawMessage
        string RawContent { get; set; }
    }
}