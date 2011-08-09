using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.IO.IsolatedStorage;
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

        private new void CreateDatabase()
        {
            try
            {
                // Generate the database (with structure) from the code-based data context
                base.CreateDatabase();

                // Populate the database with system data
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
            //TODO: Remove after testing
            this.SubmitChanges();

            var user = Factory.CreateUser();
            this.User.InsertOnSubmit(user);
            //TODO: Remove after testing
            this.SubmitChanges();

            var servers = Factory.CreateServers();
            this.Servers.InsertAllOnSubmit(servers);
            //TODO: Remove after testing
            this.SubmitChanges();

            var networks = Factory.CreateNetworks();
            this.Networks.InsertAllOnSubmit(networks);
            //TODO: Remove after testing
            this.SubmitChanges();

            var session = Factory.CreateSession();
            this.Session.InsertOnSubmit(session);
            this.SubmitChanges();
        }

        #endregion
    
        #region Table Properties

        public Table<ChannelSummary> Channels
        {
            get { return _channels ?? (_channels = GetTable<ChannelSummary>()); }
        }
        private Table<ChannelSummary> _channels;

        public Table<ChannelItem> ChannelItems
        {
            get { return _channelItems ?? (_channelItems = GetTable<ChannelItem>()); }
        }
        private Table<ChannelItem> _channelItems;

        public Table<MentionSummary> Mentions
        {
            get { return _mentions ?? (_mentions = GetTable<MentionSummary>()); }
        }
        private Table<MentionSummary> _mentions;

        public Table<MentionItem> MentionItems
        {
            get { return _mentionItems ?? (_mentionItems = GetTable<MentionItem>()); }
        }
        private Table<MentionItem> _mentionItems;

        public Table<MessageSummary> Messages
        {
            get { return _messages ?? (_messages = GetTable<MessageSummary>()); }
        }
        private Table<MessageSummary> _messages;

        public Table<MessageItem> MessageItems
        {
            get { return _messageItems ?? (_messageItems = GetTable<MessageItem>()); }
        }
        private Table<MessageItem> _messageItems;

        // Settings
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

        public Table<Settings.SessionServer> SessionServers
        {
            get { return _sessionServers ?? (_sessionServers = GetTable<Settings.SessionServer>()); }
        }
        private Table<Settings.SessionServer> _sessionServers;

        public Table<Settings.SessionNetwork> SessionNetworks
        {
            get { return _sessionNetworks ?? (_sessionNetworks = GetTable<Settings.SessionNetwork>()); }
        }
        private Table<Settings.SessionNetwork> _sessionNetworks;

        #endregion
    }
}
