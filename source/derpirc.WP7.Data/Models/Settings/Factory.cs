using System.Collections.Generic;

namespace derpirc.Data.Models.Settings
{
    public class Factory
    {
        public static Client CreateClient()
        {
            var result = new Client()
            {
                //Name = "default",
            };
            return result;
        }

        public static List<Network> CreateNetworks()
        {
            var result = new List<Network>();

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
            // Networks
            var item1 = new Network()
            {
                DisplayName = "EFNet",
                Name = "EFNet",
                HostName = "irc.efnet.org",
                Ports = "6667",
            };
            item1.Favorites.Add(channel1);
            result.Add(item1);
            var item2 = new Network()
            {
                DisplayName = "PowerPrecision",
                Name = "PowerPrecision",
                HostName = "irc.node-3.net",
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
            result.Name = "config";
            var networks = CreateNetworks();
            result.Networks.AddRange(networks);
            CleanNetworks(result);
            return result;
        }

        private static void CleanNetworks(Session session)
        {
            if (session.Networks != null)
                for (int index = 0; index < session.Networks.Count; index++)
                {
                    var record = session.Networks[index];
                    record.Id = index + 1;
                }
        }

        public static User CreateUser()
        {
            var result = new User()
            {
                NickName = "derpirc",
                NickNameAlternate = "durpirc",
                FullName = "derpirc WP7 IRC Client",
                Username = "derpirc",
                IsInvisible = true,
                QuitMessage = "I am a pretty pretty butterfly.",
            };
            return result;
        }
    }
}
