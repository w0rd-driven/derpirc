
namespace derpirc.Data.Settings
{
    public interface IServer
    {
        string DisplayName { get; set; }
        string HostName { get; set; }
        int Port { get; set; }
        string Ports { get; set; }
        string Group { get; set; }
        string Password { get; set; }
    }
}
