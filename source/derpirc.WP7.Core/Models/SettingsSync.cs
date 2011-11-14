using System;
using System.Collections.Generic;

namespace derpirc.Core
{
    public class SettingsSync
    {
        public List<string> OldNetworks { get; set; }
        public List<string> NewNetworks { get; set; }

        public List<Tuple<string, string>> OldFavorites { get; set; }

        public SettingsSync()
        {
            OldNetworks = new List<string>();
            NewNetworks = new List<string>();
            OldFavorites = new List<Tuple<string, string>>();
        }
    }
}
