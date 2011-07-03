
namespace derpirc.Data.Settings
{
    public interface INetwork
    {
        string Name { get; set; }

        // Auto Join
        bool IsJoinEnabled { get; set; }
        string JoinChannels { get; set; }
        int JoinDelay { get; set; }
    }
}
