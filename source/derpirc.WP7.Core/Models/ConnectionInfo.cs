using System.Collections.Generic;

namespace derpirc.Core
{
    public class ConnectionInfo
    {
        public string NetworkName { get; set; }
        public bool IsConnected { get; set; }
        public Dictionary<string, bool> Channels { get; set; }

        public ConnectionInfo()
        {
            this.Channels = new Dictionary<string, bool>();
        }

        public void AddOrUpdateChannel(string channelName, bool isConnected)
        {
            if (!this.Channels.ContainsKey(channelName))
                this.Channels.Add(channelName, isConnected);
            else
                this.Channels[channelName] = isConnected;
        }

        public void RemoveChannel(string channelName)
        {
            if (this.Channels.ContainsKey(channelName))
                this.Channels.Remove(channelName);
        }
    }
}
