using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using derpirc.Data.Models.Settings;

namespace derpirc.Data
{
    public class SettingsUnitOfWork : IUnitOfWork
    {
        #region Properties

        public Client Client { get; set; }
        public User User { get; set; }
        public ObservableCollection<Network> Networks { get; set; }
        public bool IsInitialized { get; set; }

        #endregion

        #region Static Impl

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
            InitializeDatabase(true);
        }

        public void WipeDatabase()
        {
            GenerateSystemData();
            Commit();
        }

        public void InitializeDatabase(bool wipe)
        {
            if (wipe)
                WipeDatabase();
            GetDefaultValues();
        }

        private void GetDefaultValues()
        {
            Client client;
            ObservableCollection<Network> networks;
            User user;

            IsolatedStorageSettings.ApplicationSettings.TryGetValue<Client>("client", out client);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<ObservableCollection<Network>>("networks", out networks);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<User>("user", out user);

            var isDataFound = false;
            if (client != null)
            {
                isDataFound = true;
                Client = client;
            }
            if (networks != null)
            {
                isDataFound = true;
                Networks = networks;
            }
            if (user != null)
            {
                isDataFound = true;
                User = user;
            }
            if (!isDataFound)
            {
                GenerateSystemData();
                Commit();
            }
            else
                IsInitialized = true;
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
                    IsolatedStorageSettings.ApplicationSettings.Add("user", Networks);
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
        }

        public void Dispose()
        {
        }
    }
}
