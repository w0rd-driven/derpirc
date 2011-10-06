using System;

namespace derpirc.Core
{
    public class ClientStatusEventArgs : EventArgs
    {
        public ClientInfo Info { get; set; }
    }
}
