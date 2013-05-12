using System;
using System.Collections.Generic;

namespace derpirc.Data.Models.Settings
{
    public class Factory
    {
        public static Client CreateClient()
        {
            var result = new Client();
            result.IsReconnectOnDisconnect = true;
            result.IsJoinOnInvite = true;
            result.IsRejoinOnKick = true;
            result.DisconnectRetries = 5;
            result.DisconnectRetryWait = 5;
            return result;
        }

        private static List<Network> CreateNetworks()
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
                DisplayName = "Freenode",
                Name = "Freenode",
                HostName = "irc.freenode.net",
                Ports = "6667",
            };
            item2.Favorites.Add(channel1);
            result.Add(item2);
            var item3 = new Network()
            {
                DisplayName = "PowerPrecision",
                Name = "PowerPrecision",
                HostName = "irc.node-3.net",
                Ports = "6667",
            };
            item3.Favorites.Add(channel2);
            item3.Favorites.Add(channel3);
            result.Add(item3);

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

        public static Storage CreateStorage()
        {
            var result = new Storage();
            result.StoreMaxMessageDays = 7;
            result.StoreMaxMessages = 1000;
            result.ShowMaxMessages = 50;
            result.IsSyncRequired = true;
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
