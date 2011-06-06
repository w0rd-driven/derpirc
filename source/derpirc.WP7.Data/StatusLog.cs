using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace derpirc.Data
{
    public class StatusLog : IMessagesView
    {
        public string Name { get; set; }
        public List<Message> Messages { get; set; }

        public StatusLog()
        {
            Messages = new List<Message>();
        }
    }
}
