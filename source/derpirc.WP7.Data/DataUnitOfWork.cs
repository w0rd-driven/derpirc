using System.Data.Linq;

namespace derpirc.Data
{
    public class DataUnitOfWork : IUnitOfWork
    {
        #region Properties

        public IRepository<ChannelSummary> Channels
        {
            get { return _channels ?? (_channels = new SqlRepository<ChannelSummary>(_context)); }
        }
        private SqlRepository<ChannelSummary> _channels = null;

        public IRepository<ChannelItem> ChannelItems
        {
            get { return _channelItems ?? (_channelItems = new SqlRepository<ChannelItem>(_context)); }
        }
        private SqlRepository<ChannelItem> _channelItems = null;

        public IRepository<MentionSummary> Mentions
        {
            get { return _mentions ?? (_mentions = new SqlRepository<MentionSummary>(_context)); }
        }
        private SqlRepository<MentionSummary> _mentions = null;

        public IRepository<MentionItem> MentionItems
        {
            get { return _mentionItems ?? (_mentionItems = new SqlRepository<MentionItem>(_context)); }
        }
        private SqlRepository<MentionItem> _mentionItems = null;

        public IRepository<MessageSummary> Messages
        {
            get { return _messages ?? (_messages = new SqlRepository<MessageSummary>(_context)); }
        }
        private SqlRepository<MessageSummary> _messages = null;

        public IRepository<MessageItem> MessageItems
        {
            get { return _messageItems ?? (_messageItems = new SqlRepository<MessageItem>(_context)); }
        }
        private SqlRepository<MessageItem> _messageItems = null;

        public IRepository<Settings.Client> Client
        {
            get { return _client ?? (_client = new SqlRepository<Settings.Client>(_context)); }
        }
        private SqlRepository<Settings.Client> _client = null;

        public IRepository<Settings.User> User
        {
            get { return _user ?? (_user = new SqlRepository<Settings.User>(_context)); }
        }
        private SqlRepository<Settings.User> _user = null;

        public IRepository<Settings.Server> Servers
        {
            get { return _servers ?? (_servers = new SqlRepository<Settings.Server>(_context)); }
        }
        private SqlRepository<Settings.Server> _servers = null;

        public IRepository<Settings.Network> Networks
        {
            get { return _networks ?? (_networks = new SqlRepository<Settings.Network>(_context)); }
        }
        private SqlRepository<Settings.Network> _networks = null;

        public IRepository<Settings.Session> Sessions
        {
            get { return _sessions ?? (_sessions = new SqlRepository<Settings.Session>(_context)); }
        }
        private SqlRepository<Settings.Session> _sessions = null;

        #endregion

        readonly DataContext _context;

        public DataUnitOfWork()
        {
            _context = new DataModelContainer();
        }

        public bool InitializeDatabase(bool wipe)
        {
            var context = (_context as DataModelContainer);
            context.InitializeDatabase(wipe);
            return context.DatabaseExists();
        }

        public void Commit()
        {
            _context.SubmitChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
