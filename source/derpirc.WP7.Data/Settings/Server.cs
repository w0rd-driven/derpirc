using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table(Name = "Servers")]
    public partial class Server : BaseModel<Server>, IServer
    {
        [Column(DbType = "Int NOT NULL", IsPrimaryKey = true)]
        public new int Id { get; set; }
        [Column(DbType = "Int")]
        public int NetworkId { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string DisplayName { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string HostName { get; set; }
        // TODO: Ports parsing
        [Column(DbType = "NVarChar(255)")]
        public string Ports { get; set; }
        [Column(DbType = "Int")]
        public int Port { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string Group { get; set; }
        [Column(DbType = "NVarChar(255)")]
        public string Password { get; set; }

        public Server()
        {

        }
    }
}
