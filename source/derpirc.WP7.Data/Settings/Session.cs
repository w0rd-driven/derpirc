using System.Collections.Generic;

namespace derpirc.Data.Settings
{
    public class Session : BaseModel<Session>, ISession
    {
        public List<Network> Networks { get; set; }
        public List<Server> Servers { get; set; }

        public Session()
        {
            Networks = new List<Network>();
            Servers = new List<Server>();
        }
    }
}
