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
        public const string ConnectionString = "isostore:/" + DatabaseFileName;
        private static MappingSource mappingSource = new AttributeMappingSource();

        #region Extensibility Method Definitions
        partial void OnCreated();
        #endregion

        #region Constructors

        public DataModelContainer() :
            base(ConnectionString, mappingSource)
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
                base.CreateDatabase();

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

            var session = Factory.CreateSession();
            this.Session.InsertOnSubmit(session);

            this.SubmitChanges();
        }

        #endregion
    
        #region Table Properties

        // TODO: Implement View entities
        //public Table<ChannelsView> Channels
        //{
        //    get { return _channels ?? (_channels = GetTable<ChannelsView>()); }
        //}
        //private Table<ChannelsView> _channels;

        //public Table<MentionsView> Mentions
        //{
        //    get { return _mentions ?? (_mentions = GetTable<MentionsView>()); }
        //}
        //private Table<MentionsView> _mentions;

        //public Table<MessagesView> Messages
        //{
        //    get { return _messages ?? (_messages = GetTable<MessagesView>()); }
        //}
        //private Table<MessagesView> _messages;

        public Table<Settings.Client> Client
        {
            get { return _client ?? (_client = GetTable<Settings.Client>()); }
        }
        private Table<Settings.Client> _client;

        public Table<Settings.User> User
        {
            get { return _user ?? (_user = GetTable<Settings.User>()); }
        }
        private Table<Settings.User> _user;

        public Table<Settings.Network> Networks
        {
            get { return _networks ?? (_networks = GetTable<Settings.Network>()); }
        }
        private Table<Settings.Network> _networks;

        public Table<Settings.Server> Servers
        {
            get { return _servers ?? (_servers = GetTable<Settings.Server>()); }
        }
        private Table<Settings.Server> _servers;

        public Table<Settings.Session> Session
        {
            get { return _session ?? (_session = GetTable<Settings.Session>()); }
        }
        private Table<Settings.Session> _session;

        #endregion
    }
}
