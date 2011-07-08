using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table]
    public partial class Client : BaseNotify, IBaseModel, IClient
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = true)]
        public bool ConnectOnStartup { get; set; }
        [Column(CanBeNull = true)]
        public bool ReconnectOnDisconnect { get; set; }
        [Column(CanBeNull = true)]
        public int DefaultPort { get; set; }
        [Column(CanBeNull = true)]
        public bool IsTimeStamped { get; set; }
        [Column(CanBeNull = true)]
        public string TimeStampFormat { get; set; }
        [Column(CanBeNull = true)]
        public int WindowBuffer { get; set; }
        [Column(CanBeNull = true)]
        public bool AutoScrollOnOutput { get; set; }

        [Column(IsVersion = true)]
        private Binary version;

        public Client()
        {
        }
    }
}
