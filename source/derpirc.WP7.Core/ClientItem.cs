using IrcDotNet;

namespace derpirc.Core
{
    public class ClientItem
    {
        public int Id { get; set; }
        public IrcClient Client { get; set; }
        public ClientState State { get; set; }

        public ClientItem()
        {
            Client = new IrcClient();
            State = ClientState.Unregistered;
        }
    }
}
