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
            result.WindowBuffer = 1000;
            result.AutoScrollOnOutput = true;
            return result;
        }

        public static List<Server> CreateServers()
        {
            var result = new List<Server>();
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                Ports = "6667",
                Group = "EFNet",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.freenode.net",
                Ports = "6667",
                Group = "Freenode",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.dal.net",
                Ports = "6667",
                Group = "DALnet",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.powerprecision.com",
                Ports = "6667",
                Group = "PowerPrecision",
            });
            result.Add(new Server()
            {
                DisplayName = "Random server",
                HostName = "irc",
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
                JoinDelay = 2,
            });
            result.Add(new Network()
            {
                Name = "Freenode",
                IsJoinEnabled = true,
                JoinChannels = "#xda-devs",
                JoinDelay = 2,
            });
            result.Add(new Network()
            {
                Name = "PowerPrecision",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
                JoinDelay = 2,
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
            // HACK: Manually link a server with a network entity
            var result = new Session();
            result.Server = new Server()
            {
                Id = 0,
                NetworkId = 0,
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                Ports = "6667",
                Group = "EFNet",
            };
            result.Network = new Network()
            {
                Id = 0,
                Name = "EFNet",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
                JoinDelay = 2,
            };
            return result;
        }
    }
}
