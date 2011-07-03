using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO.IsolatedStorage;
using System;
using derpirc.Data.Settings;

namespace derpirc.Data
{
    /// <summary>
    /// The functional concrete object context. This is just like the normal
    /// context that would be generated using the POCO artefact generator, 
    /// apart from the fact that this one implements an interface containing 
    /// the entity set properties and exposes <code>ITable</code>
    /// instances for entity set properties.
    /// </summary>
    public partial class DataModelContainer : DataContext, IDataModelContainer 
    {
        public const string DatabaseFileName = "IRC.sdf";
        private static MappingSource mappingSource = new AttributeMappingSource();

        #region Extensibility Method Definitions
        partial void OnCreated();
        #endregion

        #region Constructors

        public DataModelContainer() :
            base(DatabaseFileName, mappingSource)
        {
            OnCreated();
        }

        public DataModelContainer(string connection) :
            base(connection, mappingSource)
        {
            OnCreated();
        }

        public DataModelContainer(string connection, MappingSource mappingSource) :
            base(connection, mappingSource)
        {
            OnCreated();
        }
    
        #endregion

        #region Method overrides

        public void InitializeDatabase()
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (iso.FileExists(DatabaseFileName))
                    return;

                CreateDatabase();
            }
        }

        public void InitializeDatabase(bool wipe)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (wipe)
                    WipeDatabase(iso);
                else
                {
                    if (iso.FileExists(DatabaseFileName))
                        return;
                }

                CreateDatabase();
            }
        }

        private void CreateDatabase()
        {
            try
            {
                // Generate the database (with structure) from the code-based data context
                this.CreateDatabase();

                // Populate the database with system data
                // TODO: Factory.Create() goes here...
                GenerateSystemData();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error while creating the DB: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Error while creating the DB: " + ex.Message);
            }
        }

        public void WipeDatabase(IsolatedStorageFile iso)
        {
            if (iso.FileExists(DatabaseFileName))
                iso.DeleteFile(DatabaseFileName);
        }

        public bool DatabaseExists(bool overrideResult = false)
        {
            if (!overrideResult)
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (iso.FileExists(DatabaseFileName))
                        return true;
                    else
                        return false;
                }
            }
            else
                return false;
        }

        private void GenerateSystemData()
        {
            var client = Factory.CreateClient();
            this.Client.InsertOnSubmit(client);

            var user = Factory.CreateUser();
            this.User.InsertOnSubmit(user);

            var servers = Factory.CreateServers();
            foreach (var item in servers)
            {
                this.Servers.InsertOnSubmit(item);
            }

            var networks = Factory.CreateNetworks();
            foreach (var item in networks)
            {
                this.Networks.InsertOnSubmit(item);
            }

            this.SubmitChanges();
        }

        #endregion
    
        #region Table Properties

        //public IObjectSet<Company> Companies
        //{
        //    get { return _companies ?? (_companies = CreateObjectSet<Company>("Companies")); }
        //}
        //private ObjectSet<Company> _companies;
    
        public ITable<ChannelsView> Channels
        {
            get { return _channels ?? (_channels = GetTable<ChannelsView>()); }
        }
        private ITable<ChannelsView> _channels;

        public ITable<MentionsView> Mentions
        {
            get { return _mentions ?? (_mentions = GetTable<MentionsView>()); }
        }
        private ITable<MentionsView> _mentions;

        public ITable<MessagesView> Messages
        {
            get { return _messages ?? (_messages = GetTable<MessagesView>()); }
        }
        private ITable<MessagesView> _messages;

        public ITable<Settings.Client> Client
        {
            get { return _client ?? (_client = GetTable<Settings.Client>()); }
        }
        private ITable<Settings.Client> _client;

        public ITable<Settings.User> User
        {
            get { return _user ?? (_user = GetTable<Settings.User>()); }
        }
        private ITable<Settings.User> _user;

        public ITable<Settings.Network> Networks
        {
            get { return _networks ?? (_networks = GetTable<Settings.Network>()); }
        }
        private ITable<Settings.Network> _networks;

        public ITable<Settings.Server> Servers
        {
            get { return _servers ?? (_servers = GetTable<Settings.Server>()); }
        }
        private ITable<Settings.Server> _servers;

        public ITable<Settings.Session> Session
        {
            get { return _session ?? (_session = GetTable<Settings.Session>()); }
        }
        private ITable<Settings.Session> _session;

        #endregion
    }
}
