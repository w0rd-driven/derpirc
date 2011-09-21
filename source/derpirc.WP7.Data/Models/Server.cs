using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models
{
    [Table]
    public partial class Server : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Network> _network;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = true)]
        public string DisplayName { get; set; }
        [Column(CanBeNull = false)]
        public string HostName { get; set; }
        [Column(CanBeNull = true)]
        public string ConnectedHostName { get; set; }
        [Column(CanBeNull = true)]
        public int Port { get; set; }
        [Column(CanBeNull = true)]
        public string Ports { get; set; }
        [Column(CanBeNull = true)]
        public string Password { get; set; }

        #endregion

        #region Navigation Properties

        [Column(CanBeNull = true)]
        public int NetworkId { get; set; }
        [Association(Name = "Network_Item", ThisKey = "NetworkId", OtherKey = "Id", IsForeignKey = false)]
        public Network Network
        {
            get { return _network.Entity; }
            set
            {
                Network previousValue = _network.Entity;
                if ((previousValue != value || this._network.HasLoadedOrAssignedValue == false))
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _network.Entity = null;
                    }
                    _network.Entity = value;
                    if ((value != null))
                    {
                        NetworkId = value.Id;
                    }
                    else
                    {
                        NetworkId = default(int);
                    }
                    this.RaisePropertyChanged(() => NetworkId);
                    this.RaisePropertyChanged(() => Network);
                }
            }
        }

        #endregion

        public Server()
        {
            _network = default(EntityRef<Network>);
        }
    }
}
