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
        ITable<ChannelsView> Channels { get; }
        ITable<MentionsView> Mentions { get; }
        ITable<MessagesView> Messages { get; }
        // Settings
        ITable<Client> Client { get; }
        ITable<User> User { get; }
        ITable<Network> Networks { get; }
        ITable<Server> Servers { get; }
        ITable<Session> Session { get; }
    }
}
