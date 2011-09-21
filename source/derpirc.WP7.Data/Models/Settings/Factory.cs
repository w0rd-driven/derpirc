using System.Collections.Generic;
using System.Data.Linq;

namespace derpirc.Data.Models.Settings
{
    public class Factory
    {
        public static User CreateUser()
        {
            var result = new User()
            {
                NickName = "derpirc",
                NickNameAlternate = "durpirc",
                FullName = "derpirc WP7 IRC Client",
                Username = "derpirc",
                IsInvisible = true,
                QuitMessage = "derpirc derka muhammad jihad",
            };
            return result;
        }

        public static Formatting CreateFormatting()
        {
            var result = new Formatting()
            {
                FontFamily = "Monofur",
                FontSize = "Small",
                FontWeight = "Normal",
            };
            return result;
        }

        public static List<Server> CreateServers()
        {
            var result = new List<Server>();
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                Port = 6667,
                Ports = "6667",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.freenode.net",
                Port = 6667,
                Ports = "6667",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.dal.net",
                Port = 6667,
                Ports = "6667",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.powerprecision.com",
                Port = 6667,
                Ports = "6667",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc",
                Port = 6667,
                Ports = "6667",
            });

            return result;
        }

        public static List<Network> CreateNetworks()
        {
            var result = new List<Network>();
            result.Add(new Network()
            {
                Name = "EFNet",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
            });
            result.Add(new Network()
            {
                Name = "Freenode",
                IsJoinEnabled = true,
                JoinChannels = "#xda-devs",
            });
            result.Add(new Network()
            {
                Name = "PowerPrecision",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
            });
            return result;
        }

        public static Session CreateSession()
        {
            var result = new Session();
            result.Name = "Default";
            var servers = CreateSessionServers(result);
            var networks = CreateSessionNetworks(result, servers);
            result.Servers = servers;
            result.Networks = networks;
            return result;
        }

        public static EntitySet<SessionServer> CreateSessionServers(Session session)
        {
            var result = new EntitySet<SessionServer>();
            result.Add(new SessionServer()
            {
                Session = session,
                SessionId = session.Id,
                BasedOnId = 1,
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                Port = 6667,
                Ports = "6667",
            });
            result.Add(new SessionServer()
            {
                Session = session,
                SessionId = session.Id,
                BasedOnId = 2,
                DisplayName = "Random server",
                HostName = "irc.node-3.net",
                Port = 6667,
                Ports = "6667",
            });

            return result;
        }

        public static EntitySet<SessionNetwork> CreateSessionNetworks(Session session, EntitySet<SessionServer> servers)
        {
            var result = new EntitySet<SessionNetwork>();
            result.Add(new SessionNetwork()
            {
                Session = session,
                SessionId = session.Id,
                BasedOnId = 1,
                Server = servers[0],
                Name = "EFNet",
                IsJoinEnabled = true,
                JoinChannels = "#wp7test",
            });
            result.Add(new SessionNetwork()
            {
                Session = session,
                SessionId = session.Id,
                BasedOnId = 2,
                Server = servers[1],
                Name = "PowerPrecision",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
            });

            return result;
        }
    }
}