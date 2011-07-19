
namespace derpirc.Data.Settings
{
    public interface ISessionServer
    {
        int SessionId { get; set; }
        Session Session { get; set; }
        int BasedOnId { get; set; }
        Server Server { get; set; }
    }
}