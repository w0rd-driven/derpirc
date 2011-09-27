using System.Data.Linq;

namespace derpirc.Data.Models
{
    public class Factory
    {
        public static Session CreateSession()
        {
            var result = new Session();
            result.Name = "Default";
            var networks = CreateNetworks(result);
            result.Networks = networks;
            return result;
        }

        public static EntitySet<Network> CreateNetworks(Session session)
        {
            var result = new EntitySet<Network>();
            var server1 = new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                ConnectedHostName = "irc.efnet.org",
                Ports = "6667",
            };
            result.Add(new Network()
            {
                Session = session,
                Server = server1,
                Name = "EFNet",
                IsJoinEnabled = true,
                JoinChannels = "#wp7test",
            });
            var server2 = new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.node-3.net",
                ConnectedHostName = "irc.node-3.net",
                Ports = "6667",
            };
            result.Add(new Network()
            {
                Session = session,
                Server = server2,
                Name = "PowerPrecision",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
            });

            return result;
        }
    }
}