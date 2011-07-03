
namespace derpirc.Data.Settings
{
    public interface ISession
    {
        int NetworkId { get; set; }
        Network Network { get; set; }
        int ServerId { get; set; }
        Server Server { get; set; }
    }
}