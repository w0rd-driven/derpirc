using System;
using System.Collections.Generic;

namespace derpirc.Data
{
    public class Channel : IMessagesView
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        // Bans, Exempts, Invites
        public List<string> NickNames { get; set; }
        public List<Message> Messages { get; set; }
        public string Key { get; set; }

        public Channel()
        {
            NickNames = new List<string>();
            Messages = new List<Message>();
        }
    }
}
