using System;
using System.ComponentModel;

namespace derpirc.Core
{
    public class ClientInfo
    {
        public int Id { get; set; }
        public string NetworkName { get; set; }
        public ClientState State { get; set; }
        public Exception Error { get; set; }

        public ClientInfo()
        {
            State = ClientState.Inconceivable;
        }
    }
}
