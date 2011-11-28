using System;
using System.Collections.Generic;
using derpirc.Data.Models;

namespace derpirc.Core
{
    public class SettingsSync
    {
        public bool IsUserChanged { get; set; }
        public List<Tuple<Network, ChangeType>> Networks { get; set; }
        public List<Tuple<Favorite, ChangeType>> Favorites { get; set; }

        public SettingsSync()
        {
            Networks = new List<Tuple<Network, ChangeType>>();
            Favorites = new List<Tuple<Favorite, ChangeType>>();
        }
    }
}
