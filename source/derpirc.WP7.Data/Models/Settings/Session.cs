using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models.Settings
{
    [Table(Name = "Session")]
    public partial class Session : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntitySet<SessionServer> _servers;
        private EntitySet<SessionNetwork> _networks;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        [Association(Name = "Server_Items", ThisKey = "Id", OtherKey = "SessionId", DeleteRule = "NO ACTION")]
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

        [Association(Name = "Network_Items", ThisKey = "Id", OtherKey = "SessionId", DeleteRule = "NO ACTION")]
        public ICollection<SessionNetwork> Networks
        {
            get
            {
                return _networks;
            }
            set
            {
                if (!ReferenceEquals(_networks, value))
                {
                    var previousValue = _servers;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupNetworks;
                    }
                    _networks.SetSource(value);
                    var newValue = value as FixupCollection<SessionNetwork>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupNetworks;
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
                    item.Session = this;
            }
            if (e.OldItems != null)
            {
                foreach (SessionServer item in e.OldItems)
                {
                    if (ReferenceEquals(item.Session, this))
                        item.Session = null;
                }
            }
        }

        private void FixupNetworks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SessionNetwork item in e.NewItems)
                    item.Session = this;
            }
            if (e.OldItems != null)
            {
                foreach (SessionNetwork item in e.OldItems)
                {
                    if (ReferenceEquals(item.Session, this))
                        item.Session = null;
                }
            }
        }

        #endregion

        public Session()
        {
            Name = "Default";
            _servers = new EntitySet<SessionServer>();
            _servers.CollectionChanged += FixupServers;
            _networks = new EntitySet<SessionNetwork>();
            _networks.CollectionChanged += FixupNetworks;
        }
    }
}
