using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO.IsolatedStorage;
using System.Linq;
using derpirc.Data.Models;

namespace derpirc.Data
{
    /// <summary>
    /// The functional concrete object context. This is just like the normal
    /// context that would be generated using the POCO artefact generator, 
    /// apart from the fact that this one implements an interface containing 
    /// the entity set properties and exposes <code>ITable</code>
    /// instances for entity set properties.
    /// </summary>
    public partial class DataModelContainer : DataContext
    {
        #region Table Properties

        public Table<Channel> Channels
        {
            get { return _channels ?? (_channels = GetTable<Channel>()); }
        }
        private Table<Channel> _channels;

        public Table<ChannelItem> ChannelItems
        {
            get { return _channelItems ?? (_channelItems = GetTable<ChannelItem>()); }
        }
        private Table<ChannelItem> _channelItems;

        public Table<Mention> Mentions
        {
            get { return _mentions ?? (_mentions = GetTable<Mention>()); }
        }
        private Table<Mention> _mentions;

        public Table<MentionItem> MentionItems
        {
            get { return _mentionItems ?? (_mentionItems = GetTable<MentionItem>()); }
        }
        private Table<MentionItem> _mentionItems;

        public Table<Message> Messages
        {
            get { return _messages ?? (_messages = GetTable<Message>()); }
        }
        private Table<Message> _messages;

        public Table<MessageItem> MessageItems
        {
            get { return _messageItems ?? (_messageItems = GetTable<MessageItem>()); }
        }
        private Table<MessageItem> _messageItems;

        public Table<Session> Session
        {
            get { return _session ?? (_session = GetTable<Session>()); }
        }
        private Table<Session> _session;

        public Table<Server> Servers
        {
            get { return _servers ?? (_servers = GetTable<Server>()); }
        }
        private Table<Server> _servers;

        public Table<Network> Networks
        {
            get { return _networks ?? (_networks = GetTable<Network>()); }
        }
        private Table<Network> _networks;

        #endregion

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
            var session = Factory.CreateSession();
            this.Session.InsertOnSubmit(session);
            this.SubmitChanges();
        }

        #endregion
    }
}
