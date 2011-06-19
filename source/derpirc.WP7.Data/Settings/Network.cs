using System.Collections.Generic;

namespace derpirc.Data.Settings
{
    public class Network : BaseModel<Network>, INetwork
    {
        public string Name { get; set; }

        // TODO: Legacy: Is AutoJoin channels relevant? What about a global toggle to turn it off?
        // Auto Join
        public bool IsJoinEnabled { get; set; }
        public string JoinChannels { get; set; }
        public int JoinDelay { get; set; }

        #region TODO
        // Auto Identify
        public bool IsIdentifyEnabled { get; set; }
        /// <summary>
        /// Stored as a nickname/password pair (hopefully the password can be secured looking at 7pass source?)
        /// </summary>
        public Dictionary<string, string> IdenfityNickNames { get; set; }
        /// <summary>
        /// NickServ Identify <![CDATA[<pass>]]>
        /// </summary>
        public string IdentifyCommand { get; set; }
        // Perform
        public string PerformCommands { get; set; }
        public List<string> PerformCommandsList { get; set; }
        #endregion

        public Network()
        {

        }
    }
}
