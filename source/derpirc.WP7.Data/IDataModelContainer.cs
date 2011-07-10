using System.Data.Linq;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    /// <summary>
    /// The interface for the specialised object context. This contains all of
    /// the <code>ITable</code> properties that are implemented in both the
    /// functional context class and the mock context class.
    /// </summary>
    public interface IDataModelContainer
    {
        Table<ChannelSummary> Channels { get; }
        Table<ChannelDetail> ChannelDetails { get; }
        Table<ChannelMessage> ChannelMessages { get; }
        Table<MentionSummary> Mentions { get; }
        Table<MessageSummary> Messages { get; }
        // Settings
        Table<Client> Client { get; }
        Table<User> User { get; }
        Table<Network> Networks { get; }
        Table<Server> Servers { get; }
        Table<Session> Session { get; }
    }
}