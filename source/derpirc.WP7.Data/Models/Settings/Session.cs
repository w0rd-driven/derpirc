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
        private EntitySet<Network> _networks;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        [Association(Name = "Network_Items", ThisKey = "Id", OtherKey = "SessionId", DeleteRule = "NO ACTION")]
        public EntitySet<Network> Networks
        {
            get { return _networks; }
            set { _networks.Assign(value); }
        }

        #endregion

        public Session()
        {
            Name = "default";
            _networks = new EntitySet<Network>(new Action<Network>(attach_Networks), new Action<Network>(detach_Networks));
        }

        void attach_Networks(Network entity)
        {
            //this.RaisePropertyChanged();
            entity.Session = this;
        }

        void detach_Networks(Network entity)
        {
            //this.RaisePropertyChanged();
            entity.Session = null;
        }
    }
}
