using System;
using IrcDotNet;
using IrcDotNet.Ctcp;

namespace derpirc.Core
{
    public class ClientItem
    {
        public ClientInfo Info { get; set; }
        public IrcClient Client { get; set; }
        public CtcpClient CtcpClient { get; set; }

        public ClientItem()
        {
            Info = new ClientInfo();
            Client = new IrcClient();
            CtcpClient = new CtcpClient(Client);
        }
    }
}
