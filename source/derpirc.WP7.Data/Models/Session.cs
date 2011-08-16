using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models
{
    [Table]
    public partial class Session : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntitySet<Server> _servers;
        private EntitySet<Network> _networks;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        [Association(Name = "Server_Items", ThisKey = "Id", OtherKey = "SessionId", DeleteRule = "NO ACTION")]
        public ICollection<Server> Servers
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
                    var newValue = value as FixupCollection<Server>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupServers;
                    }
                }
            }
        }

        [Association(Name = "Network_Items", ThisKey = "Id", OtherKey = "SessionId", DeleteRule = "NO ACTION")]
        public ICollection<Network> Networks
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
                    var newValue = value as FixupCollection<Network>;
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
                foreach (Server item in e.NewItems)
                    item.Session = this;
            }
            if (e.OldItems != null)
            {
                foreach (Server item in e.OldItems)
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
                foreach (Network item in e.NewItems)
                    item.Session = this;
            }
            if (e.OldItems != null)
            {
                foreach (Network item in e.OldItems)
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
            _servers = new EntitySet<Server>();
            _servers.CollectionChanged += FixupServers;
            _networks = new EntitySet<Network>();
            _networks.CollectionChanged += FixupNetworks;
        }
    }
}
