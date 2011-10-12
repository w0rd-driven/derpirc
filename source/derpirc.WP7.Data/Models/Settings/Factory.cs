using System.Data.Linq;

namespace derpirc.Data.Models.Settings
{
    public class Factory
    {
        public static User CreateUser()
        {
            var result = new User()
            {
                Name = "default",
                NickName = "derpirc",
                NickNameAlternate = "durpirc",
                FullName = "derpirc WP7 IRC Client",
                Username = "derpirc",
                IsInvisible = true,
                QuitMessage = "I am a pretty pretty butterfly.",
            };
            return result;
        }

        public static Formatting CreateFormatting()
        {
            var result = new Formatting()
            {
                Name = "default",
                FontFamily = "Monofur",
                FontSize = "Small",
                FontWeight = "Normal",
            };
            return result;
        }

        public static Session CreateSession()
        {
            var result = new Session();
            result.Name = "default";
            var networks = CreateNetworks(result);
            result.Networks = networks;
            return result;
        }

        public static EntitySet<Network> CreateNetworks(Session session)
        {
            var result = new EntitySet<Network>();

            // Favorites
            var channel1 = new Favorite()
            {
                Name = "#wp7",
                IsAutoConnect = true,
            };
            var channel2 = new Favorite()
            {
                Name = "#xna",
                IsAutoConnect = true,
            };
            // Servers
            var server1 = new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                Ports = "6667",
            };
            var server2 = new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.node-3.net",
                Ports = "6667",
            };

            var item1 = new Network()
            {
                Session = session,
                Server = server1,
                DisplayName = "EFNet",
                Name = "EFNet",
            };
            item1.Favorites.Add(channel1);
            item1.Favorites.Add(channel2);
            result.Add(item1);

            var item2 = new Network()
            {
                Session = session,
                Server = server2,
                DisplayName = "PowerPrecision",
                Name = "PowerPrecision",
            };
            item2.Favorites.Add(channel1);
            item2.Favorites.Add(channel2);
            result.Add(item2);

            return result;
        }

        public static Storage CreateStorage()
        {
            var result = new Storage()
            {
                Name = "default",
            };
            return result;
        }
    }
}