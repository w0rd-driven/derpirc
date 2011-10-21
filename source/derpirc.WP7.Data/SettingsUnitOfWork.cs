﻿using System.Collections.Generic;
using System.IO.IsolatedStorage;
using derpirc.Data.Models.Settings;

namespace derpirc.Data
{
    public class SettingsUnitOfWork : IUnitOfWork
    {
        #region Properties

        public Client Client { get; set; }
        public User User { get; set; }
        public Session Session { get; set; }
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
            Session session;
            User user;

            IsolatedStorageSettings.ApplicationSettings.TryGetValue<Client>("client", out client);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<Session>("session", out session);
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<User>("user", out user);

            // HACK: Settings data relies on all properties being set
            var isDataFound = false;
            if (client != null && session != null && user != null)
                isDataFound = true;

            Client = client;
            Session = session;
            User = user;

            if (!isDataFound)
            {
                GenerateSystemData();
            }
            Networks = Session.Networks;

            State = DbState.Initialized;
        }

        public void Commit()
        {
            Commit(CommitType.User);
            Commit(CommitType.Session);
        }

        public void Commit(CommitType type)
        {
            switch (type)
            {
                case CommitType.User:
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
                    break;
                case CommitType.Session:
                    if (Session != null)
                    {
                        if (!IsolatedStorageSettings.ApplicationSettings.Contains("session"))
                            IsolatedStorageSettings.ApplicationSettings.Add("session", Session);
                        else
                        {
                            IsolatedStorageSettings.ApplicationSettings.Remove("session");
                            IsolatedStorageSettings.ApplicationSettings.Add("session", Session);
                        }
                    }
                    break;
                default:
                    break;
            }
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private void GenerateSystemData()
        {
            Client = Factory.CreateClient();
            User = Factory.CreateUser();
            Session = Factory.CreateSession();
            Commit();
        }
    }
}
