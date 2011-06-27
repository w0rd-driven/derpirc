
namespace derpirc.Data.Settings
{
    public interface IServer
    {
        int NetworkId { get; set; }
        string DisplayName { get; set; }
        string HostName { get; set; }
        string Ports { get; set; }
        int Port { get; set; }
        string Group { get; set; }
        string Password { get; set; }
    }
}
