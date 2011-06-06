
namespace derpirc.Data.Settings
{
    public class Server : BaseModel<Server>
    {
        public int NetworkId { get; set; }
        public string DisplayName { get; set; }
        public string HostName { get; set; }
        // TODO: Ports parsing
        public string Ports { get; set; }
        public int Port { get; set; }
        public string Group { get; set; }
        public string Password { get; set; }

        public Server()
        {

        }
    }
}
