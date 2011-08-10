using System.Data.Linq;

namespace derpirc.Data
{
    public class DataUnitOfWork : IUnitOfWork
    {
        #region Properties

        #region Tables

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

        private string _baseConnectionString;
        private string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (_connectionString == value)
                    return;

                var oldValue = _connectionString;
                _connectionString = value;
                //RaisePropertyChanged(() => Password);
            }
        }

        private string _dataSource;
        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                if (_dataSource == value)
                    return;

                var oldValue = _dataSource;
                _dataSource = value;
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value)
                    return;

                var oldValue = _password;
                _password = value;
            }
        }

        private int? _maxDatabaseSize;
        public int? MaxDatabaseSize
        {
            get { return _maxDatabaseSize; }
            set
            {
                if (_maxDatabaseSize == value)
                    return;

                var oldValue = _maxDatabaseSize;
                _maxDatabaseSize = value;
            }
        }

        private int? _maxBufferSize;
        public int? MaxBufferSize
        {
            get { return _maxBufferSize; }
            set
            {
                if (_maxBufferSize == value)
                    return;

                var oldValue = _maxBufferSize;
                _maxBufferSize = value;
            }
        }

        private FileMode _fileMode = FileMode.Default;
        public FileMode FileMode
        {
            get { return _fileMode; }
            set
            {
                if (_fileMode == value)
                    return;

                var oldValue = _fileMode;
                _fileMode = value;
            }
        }

        private bool? _caseSensitive;
        public bool? CaseSensitive
        {
            get { return _caseSensitive; }
            set
            {
                if (_caseSensitive == value)
                    return;

                var oldValue = _caseSensitive;
                _caseSensitive = value;
            }
        }

        #endregion

        //readonly DataContext _context;
        private DataContext _context;

        public DataUnitOfWork()
        {
            _baseConnectionString = "isostore:/IRC.sdf";
        }

        public bool InitializeDatabase(bool wipe)
        {
            var connectionString = GetConnectionString();
            _context = new DataModelContainer();
            var context = (_context as DataModelContainer);
            context.InitializeDatabase(wipe);
            return context.DatabaseExists();
        }

        public void Commit()
        {
            _context.SubmitChanges();
        }

        private string GetConnectionString()
        {
            var result = string.Empty;

            if (!string.IsNullOrEmpty(_connectionString))
            {
                result = _connectionString;
                return result;
            }

            var dataSource = string.Empty;
            var password = string.Empty;
            var maxDatabaseSize = string.Empty;
            var maxBufferSize = string.Empty;
            var fileMode = string.Empty;
            var culturalIdentifier = string.Empty;
            var caseSensitive = string.Empty;

            if (!string.IsNullOrEmpty(_dataSource))
                dataSource = string.Format("Data Source='{0}';", _dataSource);
            else
                dataSource = string.Format("Data Source='{0}';", _baseConnectionString);
            if (!string.IsNullOrEmpty(_password))
                password = string.Format("Password={0};", _password);
            if (_maxDatabaseSize.HasValue)
                maxDatabaseSize = string.Format("Max Database Size={0};", _maxDatabaseSize);
            if (_maxBufferSize.HasValue)
                maxBufferSize = string.Format("Max Buffer Size={0};", _maxBufferSize);

            switch (_fileMode)
            {
                case FileMode.ReadWrite:
                    fileMode = "File Mode=read write;";
                    break;
                case FileMode.ReadOnly:
                    fileMode = "File Mode=read only;";
                    break;
                case FileMode.Exclusive:
                    fileMode = "File Mode=exclusive;";
                    break;
                case FileMode.SharedRead:
                    fileMode = "File Mode=shared read;";
                    break;
            }

            if (_caseSensitive.HasValue)
                caseSensitive = string.Format("Case Sensitive={0};", _caseSensitive);

            result = dataSource + password + fileMode + maxDatabaseSize + maxBufferSize + culturalIdentifier + caseSensitive;
            return result;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
