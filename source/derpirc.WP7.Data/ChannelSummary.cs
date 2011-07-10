using System.Data.Linq;
using System.Data.Linq.Mapping;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    /// <summary>
    /// List-based Item
    /// </summary>
    [Table]
    public class ChannelSummary : BaseNotify, IBaseModel, IMessageSummary
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Server> _server;
        private EntityRef<ChannelDetail> _details;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public int ServerId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "ServerId", OtherKey = "Id", IsForeignKey = false)]
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
        [Column(CanBeNull = false)]
        public string Name { get; set; }

        [Column(CanBeNull = true)]
        public int LastItemId { get; set; }
        public IMessage LastItem { get; set; }

        // 1:0..1 with IMessageDetail
        [Column(CanBeNull = true)]
        public int DetailId { get; set; }
        [Association(Name = "Detail_Item", ThisKey = "DetailId", OtherKey = "Id", IsForeignKey = false)]
        public ChannelDetail Details
        {
            get { return _details.Entity; }
            set
            {
                ChannelDetail previousValue = _details.Entity;
                if (previousValue != value || _details.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _details.Entity = null;
                    }
                    _details.Entity = value;
                    if ((value != null))
                    {
                        DetailId = value.Id;
                    }
                    else
                    {
                        DetailId = default(int);
                    }
                    this.RaisePropertyChanged(() => DetailId);
                    this.RaisePropertyChanged(() => Details);
                }
            }
        }

        [Column(CanBeNull = true)]
        public int Count { get; set; }
        [Column(CanBeNull = true)]
        public int UnreadCount { get; set; }
        [Column(CanBeNull = true)]
        public string Topic { get; set; }

        public ChannelSummary()
        {
            _server = default(EntityRef<Server>);
            _details = default(EntityRef<ChannelDetail>);
            //TODO: Link up Detail collection events to get LastItemId, LastItem, Count, and UnreadCount
        }
    }
}