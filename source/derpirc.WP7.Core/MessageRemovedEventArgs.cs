using System;

namespace derpirc.Core
{
    public class MessageRemovedEventArgs : EventArgs
    {
        public string NetworkName { get; set; }
        public string FavoriteName { get; set; }
    }
}
