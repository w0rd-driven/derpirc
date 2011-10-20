using System.Collections.Generic;
using System.IO.IsolatedStorage;
using derpirc.Data.Models.Settings;

namespace derpirc.Data
{
    public class SettingsUnitOfWork : IUnitOfWork
    {
        #region Properties

        public Client Client { get; set; }
        public User User { get; set; }
        public List<Network> Networks { get; set; }
        public DbState State { get; set; }

        #endregion

        #region Singleton Impl

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

        #endregion

        public SettingsUnitOfWork()
        {
            //WipeDatabase();
            GetDefaultValues();
        }

        public void WipeDatabase()
        {
            State = DbState.WipePending;
            IsolatedStorageSettings.ApplicationSettings.Clear();
            GenerateSystemData();
            State = DbState.Initialized;
        }

        private void GetDefaultValues()
        {
            Client client;
            List<Network> networks;
            User user;

            IsolatedStorageSettings.ApplicationSettings.TryGetValue<Client>("client", out client);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<List<Network>>("networks", out networks);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<User>("user", out user);

            // HACK: Settings data relies on all properties being set
            var isDataFound = false;
            if (client != null && networks != null && user != null)
                isDataFound = true;

            Client = client;
            Networks = networks;
            User = user;

            if (!isDataFound)
            {
                GenerateSystemData();
            }
            State = DbState.Initialized;
        }

        public void Commit()
        {
            if (Client != null)
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("client"))
                    IsolatedStorageSettings.ApplicationSettings.Add("client", Client);
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("client");
                    IsolatedStorageSettings.ApplicationSettings.Add("client", Client);
                }
            }
            if (Networks != null)
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("networks"))
                    IsolatedStorageSettings.ApplicationSettings.Add("networks", Networks);
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("networks");
                    IsolatedStorageSettings.ApplicationSettings.Add("networks", Networks);
                }
            }
            if (User != null)
            {
                if (!IsolatedStorageSettings.ApplicationSettings.Contains("user"))
                    IsolatedStorageSettings.ApplicationSettings.Add("user", User);
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("user");
                    IsolatedStorageSettings.ApplicationSettings.Add("user", User);
                }
            }
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void GenerateSystemData()
        {
            Client = Factory.CreateClient();
            User = Factory.CreateUser();
            Networks = Factory.CreateNetworks();
            Commit();
        }
    }
}
