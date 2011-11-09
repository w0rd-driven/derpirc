using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using derpirc.Data.Models.Settings;

namespace derpirc.Data
{
    public class SettingsUnitOfWork
    {
        #region Properties

        public bool IsFactoryDefault { get; set; }

        private User _user;
        public User User
        {
            get { return _user ?? (_user = GetValueOrDefault<User>(_userKeyName, Factory.CreateUser())); }
            set
            {
                if (AddOrUpdateValue(_userKeyName, value))
                {
                    _user = value;
                    Save();
                }
            }
        }

        private List<Network> _networks;
        public List<Network> Networks
        {
            get { return _networks ?? (_networks = GetValueOrDefault<List<Network>>(_networksKeyName, Factory.CreateSession().Networks)); }
            set
            {
                if (AddOrUpdateValue(_networksKeyName, value))
                {
                    _networks = value;
                    Save();
                }
            }
        }

        private Storage _storage;
        public Storage Storage
        {
            get { return _storage ?? (_storage = GetValueOrDefault<Storage>(_storageKeyName, Factory.CreateStorage())); }
            set
            {
                if (AddOrUpdateValue(_storageKeyName, value))
                {
                    _storage = value;
                    Save();
                }
            }
        }

        private Client _client;
        public Client Client
        {
            get { return _client ?? (_client = GetValueOrDefault<Client>(_clientKeyName, Factory.CreateClient())); }
            set
            {
                if (AddOrUpdateValue(_clientKeyName, value))
                {
                    _client = value;
                    Save();
                }
            }
        }

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

        const string _userKeyName = "user";
        const string _networksKeyName = "networks";
        const string _storageKeyName = "storage";
        const string _clientKeyName = "client";

        private object _threadLock = new object();
        private IsolatedStorageSettings _settings;

        public SettingsUnitOfWork()
            : this(false)
        {
        }

        public SettingsUnitOfWork(bool wipeBeforeUse)
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;
            if (_settings.Count == 0)
                IsFactoryDefault = true;
            if (wipeBeforeUse)
                WipeDatabase();
        }

        public void WipeDatabase()
        {
            lock (_threadLock)
            {
                _settings.Clear();
                if (_settings.Count == 0)
                    IsFactoryDefault = true;
            }
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string key, object value)
        {
            lock (_threadLock)
            {
                bool valueChanged = false;

                if (_settings.Contains(key))
                {
                    if (_settings[key] != value)
                    {
                        _settings[key] = value;
                        valueChanged = true;
                    }
                }
                else
                {
                    _settings.Add(key, value);
                    valueChanged = true;
                }
                return valueChanged;
            }
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            lock (_threadLock)
            {
                T value;

                // If the key exists, retrieve the value.
                if (_settings.Contains(key))
                {
                    value = (T)_settings[key];
                    IsFactoryDefault = false;
                }
                // Otherwise, use the default value.
                else
                {
                    value = defaultValue;
                    IsFactoryDefault = true;
                }
                return value;
            }
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            try
            {
                lock (_threadLock)
                {
                    _settings.Save();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Error saving settings: " + exception);
            }
        }
    }
}
