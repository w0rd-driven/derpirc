
namespace derpirc.Data.Settings
{
    public interface ISessionNetwork
    {
        int SessionId { get; set; }
        Session Session { get; set; }
        int BasedOnId { get; set; }
        Network Network { get; set; }
    }
}