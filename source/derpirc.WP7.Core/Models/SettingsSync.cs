using System.Collections.Generic;

namespace derpirc.Core
{
    public class SettingsSync
    {
        public List<string> OldItems { get; set; }
        public List<string> NewItems { get; set; }

        public SettingsSync()
        {
            OldItems = new List<string>();
            NewItems = new List<string>();
        }
    }
}
