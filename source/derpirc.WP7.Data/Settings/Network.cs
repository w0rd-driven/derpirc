using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table(Name = "Networks")]
    public partial class Network : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string Name { get; set; }
        // Auto Join
        [Column(CanBeNull = true)]
        public bool IsJoinEnabled { get; set; }
        [Column(CanBeNull = true)]
        public string JoinChannels { get; set; }

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

        #endregion

        public Network()
        {

        }
    }
}
