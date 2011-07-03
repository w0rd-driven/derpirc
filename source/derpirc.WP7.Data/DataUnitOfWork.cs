using System.Data.Linq;

namespace derpirc.Data
{
    public class DataUnitOfWork : IUnitOfWork
    {
        readonly DataContext _context;

        SqlRepository<ChannelsView> _channels = null;
        public IRepository<ChannelsView> Channels
        {
            get
            {
                if (_channels == null)
                {
                    _channels = new SqlRepository<ChannelsView>(_context);
                }
                return _channels;
            }
        }

        SqlRepository<MentionsView> _mentions = null;
        public IRepository<MentionsView> Mentions
        {
            get
            {
                if (_mentions == null)
                {
                    _mentions = new SqlRepository<MentionsView>(_context);
                }
                return _mentions;
            }
        }

        SqlRepository<MessagesView> _messages = null;
        public IRepository<MessagesView> Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new SqlRepository<MessagesView>(_context);
                }
                return _messages;
            }
        }

        public DataUnitOfWork()
        {
            _context = new DataModelContainer();
        }

        public bool InitializeDatabase()
        {
            var context = (_context as DataModelContainer);
            context.InitializeDatabase();
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