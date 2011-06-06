using System.Collections.Generic;

namespace derpirc.Data.Settings
{
    public class Client : IClient
    {
        public bool ConnectOnStartup { get; set; }
        public bool ReconnectOnDisconnect { get; set; }
        public int DefaultPort { get; set; }
        public bool IsTimeStamped { get; set; }
        public string TimeStampFormat { get; set; }
        public int WindowBuffer { get; set; }
        public bool AutoScrollOnOutput { get; set; }

        public Client()
        {
        }
    }
}
