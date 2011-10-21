using System;
using System.Data.Linq;
using derpirc.Data.Models;

namespace derpirc.Data
{
    public class DataUnitOfWork : IUnitOfWork, IDisposable
    {
        #region Properties

        #region Tables

        public IRepository<Channel> Channels
        {
            get { return _channels ?? (_channels = new SqlRepository<Channel>(_context)); }
        }
        private SqlRepository<Channel> _channels = null;

        public IRepository<ChannelItem> ChannelItems
        {
            get { return _channelItems ?? (_channelItems = new SqlRepository<ChannelItem>(_context)); }
        }
        private SqlRepository<ChannelItem> _channelItems = null;

        public IRepository<Mention> Mentions
        {
            get { return _mentions ?? (_mentions = new SqlRepository<Mention>(_context)); }
        }
        private SqlRepository<Mention> _mentions = null;

        public IRepository<MentionItem> MentionItems
        {
            get { return _mentionItems ?? (_mentionItems = new SqlRepository<MentionItem>(_context)); }
        }
        private SqlRepository<MentionItem> _mentionItems = null;

        public IRepository<Message> Messages
        {
            get { return _messages ?? (_messages = new SqlRepository<Message>(_context)); }
        }
        private SqlRepository<Message> _messages = null;

        public IRepository<MessageItem> MessageItems
        {
            get { return _messageItems ?? (_messageItems = new SqlRepository<MessageItem>(_context)); }
        }
        private SqlRepository<MessageItem> _messageItems = null;

        public IRepository<Session> Sessions
        {
            get { return _sessions ?? (_sessions = new SqlRepository<Session>(_context)); }
        }
        private SqlRepository<Session> _sessions = null;

        public IRepository<Network> Networks
        {
            get { return _networks ?? (_networks = new SqlRepository<Network>(_context)); }
        }
        private SqlRepository<Network> _networks = null;

        #endregion

        public ContextConnectionString ConnectionString { get; set; }
        public DbState State { get; set; }

        #endregion

        private DataContext _context;

        #region Singleton Impl

        // Modified for http://www.yoda.arachsys.com/csharp/singleton.html #4. (Jon Skeet is a code machine)
        private static readonly DataUnitOfWork defaultInstance = new DataUnitOfWork();

        public static DataUnitOfWork Default
        {
            get
            {
                return defaultInstance;
            }
        }

        static DataUnitOfWork()
        {
        }

        #endregion

        public DataUnitOfWork()
            : this(new ContextConnectionString())
        {
        }

        public DataUnitOfWork(ContextConnectionString connectionString)
        {
            ConnectionString = connectionString;
            InitializeDatabase();
        }

        public void WipeDatabase()
        {
            if (State == DbState.Initialized && _context != null)
            {
                var isComplete = false;
                State = DbState.WipePending;
                var context = (_context as DataModelContainer);
                context.WipeDatabase();
                isComplete = true;
                if (isComplete)
                    InitializeDatabase();
            }
        }

        private void InitializeDatabase()
        {
            var connectionString = ConnectionString.ToString();
            if (_context == null)
                _context = new DataModelContainer(connectionString);
            var context = (_context as DataModelContainer);
            context.InitializeDatabase();
            //GenerateSystemData();
            State = DbState.Initialized;
        }

        public void Commit()
        {
            try
            {
                _context.SubmitChanges();
            }
            catch (System.InvalidOperationException)
            {

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void GenerateSystemData()
        {
            var session = Factory.CreateSession();
            this.Sessions.Add(session);
            this.Commit();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
