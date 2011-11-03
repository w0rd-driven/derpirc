using System;

namespace derpirc.Core
{
    public class NetworkAvailableEventArgs : EventArgs
    {
        public bool IsAvailable { get; private set; }

        public NetworkAvailableEventArgs(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
    }
}
