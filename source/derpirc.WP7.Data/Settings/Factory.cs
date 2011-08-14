using System.Collections.Generic;

namespace derpirc.Data.Settings
{
    public class Factory
    {
        public static Client CreateClient()
        {
            var result = new Client();
            result.ConnectOnStartup = false;
            result.ReconnectOnDisconnect = true;
            result.DefaultPort = 6667;
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
                Group = "EFNet",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.freenode.net",
                Port = 6667,
                Ports = "6667",
                Group = "Freenode",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.dal.net",
                Port = 6667,
                Ports = "6667",
                Group = "DALnet",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.powerprecision.com",
                Port = 6667,
                Ports = "6667",
                Group = "PowerPrecision",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc",
                Port = 6667,
                Ports = "6667",
                Group = "Fake",
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

        public static User CreateUser()
        {
            var nickNameAlt = new List<string>();
            var result = new User()
            {
                Name = "WP7 IRC Client",
                Email = "derpirc@urmom.com",
                NickName = "derpirc",
                // TODO: Sync CSV and ObservableCollection
                NickNameAlternates = "dERca, dERka",
                QuitMessage = "derpirc derka muhammad jihad",
            };
            return result;
        }

        public static Session CreateSession()
        {
            var result = new Session();
            result.Name = "Default";
            result.Servers = CreateSessionServers(result);
            result.Networks = CreateSessionNetworks(result);
            return result;
        }

        public static List<SessionServer> CreateSessionServers(Session session)
        {
            var result = new List<SessionServer>();
            result.Add(new SessionServer()
            {
                Session = session,
                SessionId = session.Id,
                BasedOnId = 1,
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                Port = 6667,
                Ports = "6667",
                Group = "EFNet",
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
                Group = "PowerPrecision",
            });

            return result;
        }

        public static List<SessionNetwork> CreateSessionNetworks(Session session)
        {
            var result = new List<SessionNetwork>();
            result.Add(new SessionNetwork()
            {
                Session = session,
                SessionId = session.Id,
                BasedOnId = 1,
                Name = "EFNet",
                IsJoinEnabled = true,
                JoinChannels = "#wp7test",
            });
            result.Add(new SessionNetwork()
            {
                Session = session,
                SessionId = session.Id,
                BasedOnId = 2,
                Name = "PowerPrecision",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
            });

            return result;
        }
    }
}