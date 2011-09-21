using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models.Settings
{
    [Table]
    public partial class Network : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Session> _session;
        private EntityRef<Server> _server;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }
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

        #region Navigation Properties

        [Column(CanBeNull = false)]
        public int SessionId { get; set; }
        [Association(Name = "Session_Item", ThisKey = "SessionId", OtherKey = "Id", IsForeignKey = true)]
        public Session Session
        {
            get { return _session.Entity; }
            set
            {
                Session previousValue = _session.Entity;
                if (previousValue != value || _session.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _session.Entity = null;
                    }
                    _session.Entity = value;
                    if ((value != null))
                    {
                        SessionId = value.Id;
                    }
                    else
                    {
                        SessionId = default(int);
                    }
                    this.RaisePropertyChanged(() => SessionId);
                    this.RaisePropertyChanged(() => Session);
                }
            }
        }

        [Column(CanBeNull = false)]
        public int ServerId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "ServerId", OtherKey = "Id", IsForeignKey = true)]
        public Server Server
        {
            get { return _server.Entity; }
            set
            {
                Server previousValue = _server.Entity;
                if (previousValue != value || _server.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _server.Entity = null;
                    }
                    _server.Entity = value;
                    if ((value != null))
                    {
                        ServerId = value.Id;
                    }
                    else
                    {
                        ServerId = default(int);
                    }
                    this.RaisePropertyChanged(() => ServerId);
                    this.RaisePropertyChanged(() => Server);
                }
            }
        }

        #endregion

        public Network()
        {
            _session = default(EntityRef<Session>);
            _server = default(EntityRef<Server>);
        }
    }
}
