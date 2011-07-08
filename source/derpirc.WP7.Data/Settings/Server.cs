using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Settings
{
    [Table(Name = "Servers")]
    public partial class Server : BaseNotify, IBaseModel, IServer
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column(CanBeNull = false)]
        public int NetworkId { get; set; }
        [Column(CanBeNull = true)]
        public string DisplayName { get; set; }
        [Column(CanBeNull = false)]
        public string HostName { get; set; }
        // TODO: Ports parsing
        [Column(CanBeNull = true)]
        public string Ports { get; set; }
        [Column(CanBeNull = true)]
        public int Port { get; set; }
        [Column(CanBeNull = true)]
        public string Group { get; set; }
        [Column(CanBeNull = true)]
        public string Password { get; set; }

        [Column(IsVersion = true)]
        private Binary version;

        public Server()
        {

        }
    }
}
