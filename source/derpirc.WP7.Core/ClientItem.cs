using System;
using GalaSoft.MvvmLight;
using IrcDotNet;

namespace derpirc.Core
{
    public class ClientItem
    {
        public ClientInfo Info { get; set; }
        public IrcClient Client { get; set; }

        public ClientItem()
        {
            Info = new ClientInfo();
            Client = new IrcClient();
        }
    }
}
