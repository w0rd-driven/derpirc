

namespace derpirc.Data.Settings
{
    public interface IClient
    {
        bool ConnectOnStartup { get; set; }
        bool ReconnectOnDisconnect { get; set; }
        int DefaultPort { get; set; }
        bool IsTimeStamped { get; set; }
        string TimeStampFormat { get; set; }
        int WindowBuffer { get; set; }
        bool AutoScrollOnOutput { get; set; }
    }
}
