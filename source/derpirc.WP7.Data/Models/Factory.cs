using System.Collections.Generic;

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

        public static List<Server> CreateServers(Session session)
        {
            var result = new List<Server>();
            result.Add(new Server()
            {
                Session = session,
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                Port = 6667,
                Ports = "6667",
                Group = "EFNet",
            });
            result.Add(new Server()
            {
                Session = session,
                DisplayName = "Random server",
                HostName = "irc.node-3.net",
                Port = 6667,
                Ports = "6667",
                Group = "PowerPrecision",
            });

            return result;
        }

        public static List<Network> CreateNetworks(Session session, List<Server> servers)
        {
            var result = new List<Network>();
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