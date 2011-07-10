using System;
using System.Collections.Generic;

namespace derpirc.Data
{
    public interface IMessage
    {
        // Foreign keys
        // 1:1 with IMessageSummary
        int ListId { get; set; }

        DateTime TimeStamp { get; set; }
        bool IsRead { get; set; }

        // IrcDotNet.IrcRawMessageEventArgs.Message
        // IrcDotNet.IrcClient.IrcMessage
        string Command { get; set; }
        IList<string> Parameters { get; set; }
        string Prefix { get; set; }
        // IrcDotNet.IIrcMessageSource
        string Source { get; set; }

        // IrcDotNet.IrcRawMessageEventArgs.RawMessage
        string RawContent { get; set; }
    }
}