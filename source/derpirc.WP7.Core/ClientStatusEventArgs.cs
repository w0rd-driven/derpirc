using System;

namespace derpirc.Core
{
    public class ClientStatusEventArgs : EventArgs
    {
        public ClientItem Client { get; set; }
    }
}
