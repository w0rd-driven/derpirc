using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace derpirc.Data.Settings
{
    [Table(Name = "SessionNetworks")]
    public partial class SessionNetwork : BaseNotify, IBaseModel, ISessionNetwork, INetwork
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Session> _session;
        private EntityRef<Network> _network;
        private EntitySet<SessionServer> _servers;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }
        [Column(CanBeNull = true)]
        public bool IsJoinEnabled { get; set; }
        [Column(CanBeNull = true)]
        public string JoinChannels { get; set; }
        [Column(CanBeNull = true)]
        public int JoinDelay { get; set; }

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

        [Column(CanBeNull = true)]
        public int BasedOnId { get; set; }
        [Association(Name = "Network_Item", ThisKey = "BasedOnId", OtherKey = "Id", IsForeignKey = true)]
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
                        this.BasedOnId = value.Id;
                    }
                    else
                    {
                        this.BasedOnId = default(int);
                    }
                    this.RaisePropertyChanged(() => BasedOnId);
                    this.RaisePropertyChanged(() => Network);
                }
            }
        }

        [Association(Name = "Server_Items", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
        public ICollection<SessionServer> Servers
        {
            get
            {
                return _servers;
            }
            set
            {
                if (!ReferenceEquals(_servers, value))
                {
                    var previousValue = _servers;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupServers;
                    }
                    _servers.SetSource(value);
                    var newValue = value as FixupCollection<SessionServer>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupServers;
                    }
                }
            }
        }

        #endregion

        #region Association Fixup

        private void FixupServers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SessionServer item in e.NewItems)
                    item.Network = this;
            }
            if (e.OldItems != null)
            {
                foreach (SessionServer item in e.OldItems)
                {
                    if (ReferenceEquals(item.Network, this))
                        item.Network = null;
                }
            }
        }

        #endregion

        public SessionNetwork()
        {
            _session = default(EntityRef<Session>);
            _network = default(EntityRef<Network>);
            _servers = new EntitySet<SessionServer>();
            _servers.CollectionChanged += FixupServers;
        }
    }
}
