using System.Data.Linq;

namespace derpirc.Data.Models
{
    public class Factory
    {
        public static EntitySet<Network> CreateNetworks()
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
            var item1 = new Network()
            {
                DisplayName = "EFNet",
                Name = "EFNet",
                HostName = "irc.efnet.org",
                ConnectedHostName = "irc.efnet.org",
                Ports = "6667",
            };
            item1.Favorites.Add(channel1);
            result.Add(item1);

            var item2 = new Network()
            {
                DisplayName = "PowerPrecision",
                Name = "PowerPrecision",
                HostName = "irc.node-3.net",
                ConnectedHostName = "irc.node-3.net",
                Ports = "6667",
            };
            item2.Favorites.Add(channel2);
            item2.Favorites.Add(channel3);
            result.Add(item2);

            return result;
        }

        public static Session CreateSession()
        {
            var result = new Session();
            result.Name = "default";
            var networks = CreateNetworks();
            result.Networks.AddRange(networks);
            return result;
        }
    }
}
