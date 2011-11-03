using System;

namespace derpirc.Core
{
    public class NetworkStatusEventArgs : EventArgs
    {
        public bool IsAvailable { get; private set; }
        public NetworkType Type { get; private set; }

        public NetworkStatusEventArgs(bool isAvailable, NetworkType type)
        {
            IsAvailable = isAvailable;
            Type = type;
        }
    }
}
