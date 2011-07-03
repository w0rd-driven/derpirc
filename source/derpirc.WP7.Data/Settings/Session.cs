using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table(Name = "Sessions")]
    public partial class Session : BaseModel<Session>, ISession
    {
        [Column(DbType = "Int NOT NULL", IsPrimaryKey = true)]
        public new int Id { get; set; }
        [Column(DbType = "Int")]
        public int NetworkId { get; set; }
        private EntityRef<Network> _network;
        [Association(Name = "Network_Item", ThisKey = "NetworkId", OtherKey = "Id", IsForeignKey = true)]
        public Network Network
        {
            get
            {
                return this._network.Entity;
            }
            set
            {
                Network previousValue = this._network.Entity;
                if (previousValue != value || this._network.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        this._network.Entity = null;
                    }
                    this._network.Entity = value;
                    if ((value != null))
                    {
                        this.NetworkId = value.Id;
                    }
                    else
                    {
                        this.NetworkId = default(int);
                    }
                    this.RaisePropertyChanged(() => NetworkId);
                    this.RaisePropertyChanged(() => Network);
                }
            }
        }
        [Column(DbType = "Int")]
        public int ServerId { get; set; }
        private EntityRef<Server> _server;
        [Association(Name = "Server_Item", ThisKey = "ServerId", OtherKey = "Id", IsForeignKey = true)]
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
                        this.ServerId = value.Id;
                    }
                    else
                    {
                        this.ServerId = default(int);
                    }
                    this.RaisePropertyChanged(() => ServerId);
                    this.RaisePropertyChanged(() => Server);
                }
            }
        }

        public Session()
        {
        }
    }
}
