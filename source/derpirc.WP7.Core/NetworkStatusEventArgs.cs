using System;

namespace derpirc.Core
{
    public class NetworkStatusEventArgs : EventArgs
    {
        public bool IsAvailable { get; set; }
        public NetworkType Type { get; set; }
    }
}
