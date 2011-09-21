using System.Data.Linq;

namespace derpirc.Data.Models
{
    public class Factory
    {
        public static Session CreateSession()
        {
            var result = new Session();
            result.Name = "Default";
            var servers = CreateServers(result);
            var networks = CreateNetworks(result, servers);
            result.Servers = servers;
            result.Networks = networks;
            return result;
        }

        public static EntitySet<Server> CreateServers(Session session)
        {
            var result = new EntitySet<Server>();
            result.Add(new Server()
            {
                Session = session,
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                Port = 6667,
                Ports = "6667",
            });
            result.Add(new Server()
            {
                Session = session,
                DisplayName = "Random server",
                HostName = "irc.node-3.net",
                Port = 6667,
                Ports = "6667",
            });

            return result;
        }

        public static EntitySet<Network> CreateNetworks(Session session, EntitySet<Server> servers)
        {
            var result = new EntitySet<Network>();
            result.Add(new Network()
            {
                Session = session,
                Server = servers[0],
                Name = "EFNet",
                IsJoinEnabled = true,
                JoinChannels = "#wp7test",
            });
            result.Add(new Network()
            {
                Session = session,
                Server = servers[1],
                Name = "PowerPrecision",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
            });

            return result;
        }
    }
}