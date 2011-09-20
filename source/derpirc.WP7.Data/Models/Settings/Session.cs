using System;
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
        public EntitySet<SessionServer> Servers
        {
            get { return _servers; }
            set { _servers.Assign(value); }
        }

        [Association(Name = "Network_Items", ThisKey = "Id", OtherKey = "SessionId", DeleteRule = "NO ACTION")]
        public EntitySet<SessionNetwork> Networks
        {
            get { return _networks; }
            set { _networks.Assign(value); }
        }

        #endregion

        public Session()
        {
            Name = "Default";
            _servers = new EntitySet<SessionServer>(new Action<SessionServer>(attach_Servers), new Action<SessionServer>(detach_Servers));
            _networks = new EntitySet<SessionNetwork>(new Action<SessionNetwork>(attach_Networks), new Action<SessionNetwork>(detach_Networks));
        }

        void attach_Servers(SessionServer entity)
        {
            //this.RaisePropertyChanged();
            entity.Session = this;
        }

        void detach_Servers(SessionServer entity)
        {
            //this.RaisePropertyChanged();
            entity.Session = null;
        }

        void attach_Networks(SessionNetwork entity)
        {
            //this.RaisePropertyChanged();
            entity.Session = this;
        }

        void detach_Networks(SessionNetwork entity)
        {
            //this.RaisePropertyChanged();
            entity.Session = null;
        }
    }
}
