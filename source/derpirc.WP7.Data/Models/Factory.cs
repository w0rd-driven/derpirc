using System.Data.Linq;

namespace derpirc.Data.Models
{
    public class Factory
    {
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
                Name = "#wp7test",
                IsAutoConnect = true,
            };
            var channel2 = new Favorite()
            {
                Name = "#wp7",
                IsAutoConnect = true,
            };
            var channel3 = new Favorite()
            {
                Name = "#xna",
                IsAutoConnect = true,
            };
            // Servers
            var server1 = new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.efnet.org",
                ConnectedHostName = "irc.efnet.org",
                Ports = "6667",
            };
            var server2 = new Server()
            {
                DisplayName = "Random server",
                HostName = "irc.node-3.net",
                ConnectedHostName = "irc.node-3.net",
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
            result.Add(item1);

            var item2 = new Network()
            {
                Session = session,
                Server = server2,
                DisplayName = "PowerPrecision",
                Name = "PowerPrecision",
            };
            item2.Favorites.Add(channel2);
            item2.Favorites.Add(channel3);
            result.Add(item2);

            return result;
        }
    }
}