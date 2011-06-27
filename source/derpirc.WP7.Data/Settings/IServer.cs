
namespace derpirc.Data.Settings
{
    public interface IServer
    {
        public int NetworkId { get; set; }
        public string DisplayName { get; set; }
        public string HostName { get; set; }
        public string Ports { get; set; }
        public int Port { get; set; }
        public string Group { get; set; }
        public string Password { get; set; }
    }
}
