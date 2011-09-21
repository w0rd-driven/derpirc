using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO.IsolatedStorage;
using System.Linq;
using derpirc.Data.Models.Settings;

namespace derpirc.Data
{
    /// <summary>
    /// The functional concrete object context. This is just like the normal
    /// context that would be generated using the POCO artefact generator, 
    /// apart from the fact that this one implements an interface containing 
    /// the entity set properties and exposes <code>ITable</code>
    /// instances for entity set properties.
    /// </summary>
    public partial class SettingsModelContainer : DataContext
    {
        public const string DatabaseFileName = "Settings.sdf";
        public const string ConnectionString = "isostore:/" + DatabaseFileName;
        private static MappingSource mappingSource = new AttributeMappingSource();

        #region Extensibility Method Definitions
        partial void OnCreated();
        #endregion

        #region Constructors

        public SettingsModelContainer() : base(ConnectionString, mappingSource)
        {
            OnCreated();
        }

        public SettingsModelContainer(string connection) : base(connection, mappingSource)
        {
            OnCreated();
        }

        public SettingsModelContainer(string connection, MappingSource mappingSource) :
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
            var user = Factory.CreateUser();
            this.User.InsertOnSubmit(user);
            this.SubmitChanges();

            var formatting = Factory.CreateFormatting();
            this.Formatting.InsertOnSubmit(formatting);
            this.SubmitChanges();

            var servers = Factory.CreateServers();
            this.Servers.InsertAllOnSubmit(servers);
            this.SubmitChanges();

            var networks = Factory.CreateNetworks();
            this.Networks.InsertAllOnSubmit(networks);
            this.SubmitChanges();

            var session = Factory.CreateSession();
            this.Session.InsertOnSubmit(session);
            this.SubmitChanges();

            // HACK: Fix up special Factory.Create case where networks and servers aren't linked to their parents
            var sessionNetworks = session.Networks.ToList();
            sessionNetworks.ForEach(item =>
            {
                if (item.Network == null)
                {
                    var basedOnNetwork = GetBasedOnNetwork(networks, item.BasedOnId);
                    if (basedOnNetwork != null)
                    {
                        item.Network = basedOnNetwork;
                    }
                }
            });
            this.SubmitChanges();

            var sessionServers = session.Servers.ToList();
            sessionServers.ForEach(item =>
            {
                if (item.Server == null)
                {
                    var basedOnServer = GetBasedOnServer(servers, item.BasedOnId);
                    if (basedOnServer != null)
                    {
                        item.Server = basedOnServer;
                    }
                }
            });
            this.SubmitChanges();
        }

        private Server GetBasedOnServer(List<Server> servers, int id)
        {
            return servers.Where(x => x.Id == id).FirstOrDefault();
        }

        private Network GetBasedOnNetwork(List<Network> networks, int id)
        {
            return networks.Where(x => x.Id == id).FirstOrDefault();
        }
        #endregion
    
        #region Table Properties

        public Table<User> User
        {
            get { return _user ?? (_user = GetTable<User>()); }
        }
        private Table<User> _user;

        public Table<Formatting> Formatting
        {
            get { return _formatting ?? (_formatting = GetTable<Formatting>()); }
        }
        private Table<Formatting> _formatting;

        public Table<Network> Networks
        {
            get { return _networks ?? (_networks = GetTable<Network>()); }
        }
        private Table<Network> _networks;

        public Table<Server> Servers
        {
            get { return _servers ?? (_servers = GetTable<Server>()); }
        }
        private Table<Server> _servers;

        public Table<Session> Session
        {
            get { return _session ?? (_session = GetTable<Session>()); }
        }
        private Table<Session> _session;

        public Table<SessionServer> SessionServers
        {
            get { return _sessionServers ?? (_sessionServers = GetTable<SessionServer>()); }
        }
        private Table<SessionServer> _sessionServers;

        public Table<SessionNetwork> SessionNetworks
        {
            get { return _sessionNetworks ?? (_sessionNetworks = GetTable<SessionNetwork>()); }
        }
        private Table<SessionNetwork> _sessionNetworks;

        #endregion
    }
}
