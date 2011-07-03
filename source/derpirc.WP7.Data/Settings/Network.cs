using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table(Name = "Networks")]
    public partial class Network : BaseModel<Network>, INetwork
    {
        [Column(DbType = "Int NOT NULL", IsPrimaryKey = true)]
        public new int Id { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string Name { get; set; }
        // TODO: Legacy: Is AutoJoin channels relevant? What about a global toggle to turn it off?
        // Auto Join
        [Column(DbType = "Bit")]
        public bool IsJoinEnabled { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string JoinChannels { get; set; }
        [Column(DbType = "Int")]
        public int JoinDelay { get; set; }

        #region TODO
        //// Auto Identify
        //public bool IsIdentifyEnabled { get; set; }
        ///// <summary>
        ///// Stored as a nickname/password pair (hopefully the password can be secured looking at 7pass source?)
        ///// </summary>
        //public Dictionary<string, string> IdenfityNickNames { get; set; }
        ///// <summary>
        ///// NickServ Identify <![CDATA[<pass>]]>
        ///// </summary>
        //public string IdentifyCommand { get; set; }
        //// Perform
        //public string PerformCommands { get; set; }
        //public List<string> PerformCommandsList { get; set; }
        #endregion

        public Network()
        {

        }
    }
}
