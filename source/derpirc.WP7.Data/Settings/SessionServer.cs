using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table(Name = "SessionServers")]
    public partial class SessionServer : BaseNotify, IBaseModel, ISessionServer, IServer
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Session> _session;
        private EntityRef<Server> _server;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public int NetworkId { get; set; }
        [Column(CanBeNull = true)]
        public string DisplayName { get; set; }
        [Column(CanBeNull = false)]
        public string HostName { get; set; }
        [Column(CanBeNull = true)]
        public string Ports { get; set; }
        [Column(CanBeNull = true)]
        public int Port { get; set; }
        [Column(CanBeNull = true)]
        public string Group { get; set; }
        [Column(CanBeNull = true)]
        public string Password { get; set; }

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
        public int BasedOnId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "BasedOnId", OtherKey = "Id", IsForeignKey = true)]
        public Server Server
        {
            get
            {
                return this._server.Entity;
            }
            set
            {
                Server previousValue = this._server.Entity;
                if (previousValue != value || this._server.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        this._server.Entity = null;
                    }
                    this._server.Entity = value;
                    if ((value != null))
                    {
                        this.BasedOnId = value.Id;
                    }
                    else
                    {
                        this.BasedOnId = default(int);
                    }
                    this.RaisePropertyChanged(() => BasedOnId);
                    this.RaisePropertyChanged(() => Server);
                }
            }
        }

        #endregion

        public SessionServer()
        {
            _session = default(EntityRef<Session>);
            _server = default(EntityRef<Server>);
        }
    }
}