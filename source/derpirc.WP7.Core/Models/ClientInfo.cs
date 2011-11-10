using System;
using System.ComponentModel;

namespace derpirc.Core
{
    public class ClientInfo
    {
        public string NetworkName { get; set; }
        public ClientState State { get; set; }
        public Exception Error { get; set; }
        public int ConnectionRetryCount { get; set; }
        public int NickNameRetryCount { get; set; }

        public ClientInfo()
        {
        }
    }
}
