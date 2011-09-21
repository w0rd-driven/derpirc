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
                Port = 6667,
                Ports = "6667",
            };
            result.Add(new Network()
            {
                Session = session,
                SessionId = session.Id,
                Server = server1,
                Name = "EFNet",
                IsJoinEnabled = true,
                JoinChannels = "#wp7test",
            });
            var server2 = new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.node-3.net",
                Port = 6667,
                Ports = "6667",
            };
            result.Add(new Network()
            {
                Session = session,
                SessionId = session.Id,
                Server = server2,
                Name = "PowerPrecision",
                IsJoinEnabled = true,
                JoinChannels = "#wp7, #xna",
            });

            return result;
        }
    }
}