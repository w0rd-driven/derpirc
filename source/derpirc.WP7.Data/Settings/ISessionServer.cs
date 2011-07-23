
namespace derpirc.Data.Settings
{
    public interface ISessionServer
    {
        int BasedOnId { get; set; }
        Server Server { get; set; }
        int SessionId { get; set; }
        Session Session { get; set; }
        int NetworkId { get; set; }
        SessionNetwork Network { get; set; }
    }
}