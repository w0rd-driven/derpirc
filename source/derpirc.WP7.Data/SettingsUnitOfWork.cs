using System.Data.Linq;
using derpirc.Data.Models.Settings;

namespace derpirc.Data
{
    public class SettingsUnitOfWork : IUnitOfWork
    {
        #region Properties

        #region Tables

        public IRepository<User> User
        {
            get { return _user ?? (_user = new SqlRepository<User>(_context)); }
        }
        private SqlRepository<User> _user = null;

        public IRepository<Formatting> Formatting
        {
            get { return _formatting ?? (_formatting = new SqlRepository<Formatting>(_context)); }
        }
        private SqlRepository<Formatting> _formatting = null;

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

        public IRepository<Server> Servers
        {
            get { return _servers ?? (_servers = new SqlRepository<Server>(_context)); }
        }
        private SqlRepository<Server> _servers = null;

        #endregion

        public bool DatabaseExists { get; set; }
        public ContextConnectionString ConnectionString { get; set; }

        #endregion

        //readonly DataContext _context;
        private DataContext _context;

        // Modified for http://www.yoda.arachsys.com/csharp/singleton.html #4. (Jon Skeet is a code machine)
        private static readonly SettingsUnitOfWork defaultInstance = new SettingsUnitOfWork();

        public static SettingsUnitOfWork Default
        {
            get
            {
                return defaultInstance;
            }
        }

        static SettingsUnitOfWork()
        {
        }

        public SettingsUnitOfWork()
        {
            ConnectionString = new ContextConnectionString()
            {
                ConnectionString = "isostore:/Settings.sdf",
            };
            InitializeDatabase(false);
        }

        public void WipeDatabase()
        {
            if (_context != null)
                _context.DeleteDatabase();
        }

        public void InitializeDatabase(bool wipe)
        {
            var connectionString = ConnectionString.ToString();
            if (_context == null)
                _context = new SettingsModelContainer(connectionString);
            var context = (_context as SettingsModelContainer);
            context.InitializeDatabase(wipe);
            DatabaseExists = context.DatabaseExists();
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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
