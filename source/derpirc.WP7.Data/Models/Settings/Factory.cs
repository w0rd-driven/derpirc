using System.Collections.ObjectModel;

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
                QuitMessage = "I am a pretty pretty butterfly.",
            };
            return result;
        }

        public static ObservableCollection<Network> CreateNetworks()
        {
            var result = new ObservableCollection<Network>();

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
            // Networks
            var item1 = new Network()
            {
                Id = 1,
                DisplayName = "EFNet",
                Name = "EFNet",
                HostName = "irc.efnet.org",
                Ports = "6667",
            };
            item1.Favorites.Add(channel1);
            item1.Favorites.Add(channel2);
            result.Add(item1);
            var item2 = new Network()
            {
                Id = 2,
                DisplayName = "PowerPrecision",
                Name = "PowerPrecision",
                HostName = "irc.node-3.net",
                Ports = "6667",
            };
            item2.Favorites.Add(channel1);
            item2.Favorites.Add(channel2);
            result.Add(item2);

            return result;
        }

        public static Client CreateClient()
        {
            var result = new Client()
            {
                //Name = "default",
            };
            return result;
        }
    }
}