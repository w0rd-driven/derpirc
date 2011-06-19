
namespace derpirc.Data.Settings
{
    public class INetwork
    {
        public string Name { get; set; }

        // TODO: Legacy: Is AutoJoin channels relevant? What about a global toggle to turn it off?
        // Auto Join
        public bool IsJoinEnabled { get; set; }
        public string JoinChannels { get; set; }
        public int JoinDelay { get; set; }
    }
}
