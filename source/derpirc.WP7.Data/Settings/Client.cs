using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table]
    public partial class Client : BaseModel<Client>, IClient
    {
        [Column(DbType = "Int NOT NULL", IsPrimaryKey = true)]
        public new int Id { get; set; }
        [Column(DbType = "Bit")]
        public bool ConnectOnStartup { get; set; }
        [Column(DbType = "Bit")]
        public bool ReconnectOnDisconnect { get; set; }
        [Column(DbType = "Int")]
        public int DefaultPort { get; set; }
        [Column(DbType = "Bit")]
        public bool IsTimeStamped { get; set; }
        [Column(DbType = "Bit")]
        public string TimeStampFormat { get; set; }
        [Column(DbType = "Int")]
        public int WindowBuffer { get; set; }
        [Column(DbType = "Bit")]
        public bool AutoScrollOnOutput { get; set; }

        public Client()
        {
        }
    }
}
