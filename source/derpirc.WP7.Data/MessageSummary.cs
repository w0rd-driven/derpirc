using System.Data.Linq;
using System.Data.Linq.Mapping;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    [Table]
    public class MessageSummary : BaseNotify, IBaseModel, IMessageSummary
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<SessionServer> _server;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column(CanBeNull = true)]
        public int LastItemId { get; set; }
        public IMessageItem LastItem { get; set; }

        [Column(CanBeNull = true)]
        public int Count { get; set; }
        [Column(CanBeNull = true)]
        public int UnreadCount { get; set; }

        #endregion

        #region Navigation Properties

        [Column(CanBeNull = false)]
        public int ServerId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "ServerId", OtherKey = "Id", IsForeignKey = false)]
        public SessionServer Server
        {
            get
            {
                return this._server.Entity;
            }
            set
            {
                SessionServer previousValue = this._server.Entity;
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

        #endregion

        public MessageSummary()
        {
            _server = default(EntityRef<SessionServer>);
        }
    }
}
