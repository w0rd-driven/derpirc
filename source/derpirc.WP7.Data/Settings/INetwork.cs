
namespace derpirc.Data.Settings
{
    public interface INetwork
    {
        string Name { get; set; }

        // TODO: Legacy: Is AutoJoin channels relevant? What about a global toggle to turn it off?
        // Auto Join
        bool IsJoinEnabled { get; set; }
        string JoinChannels { get; set; }
        int JoinDelay { get; set; }
    }
}
