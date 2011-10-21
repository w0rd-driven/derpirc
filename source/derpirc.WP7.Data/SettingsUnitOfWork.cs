﻿using System.Collections.Generic;
using System.IO.IsolatedStorage;
using derpirc.Data.Models.Settings;

namespace derpirc.Data
{
    public class SettingsUnitOfWork
    {
        const string _userKeyName = "user";
        const string _sessionKeyName = "session";

        private IsolatedStorageSettings _settings;

        #region Properties

        public User User
        {
            get
            {
                return GetValueOrDefault<User>(_userKeyName, Factory.CreateUser());
            }
            set
            {
                if (AddOrUpdateValue(_userKeyName, value))
                {
                    Save();
                }
            }
        }

        public Session Session
        {
            get
            {
                return GetValueOrDefault<Session>(_sessionKeyName, Factory.CreateSession());
            }
            set
            {
                if (AddOrUpdateValue(_sessionKeyName, value))
                {
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

        public SettingsUnitOfWork() : this(false)
        {
        }

        public SettingsUnitOfWork(bool wipeBeforeUse)
        {
            _settings = IsolatedStorageSettings.ApplicationSettings;
            if (wipeBeforeUse)
                WipeDatabase();
        }

        public void WipeDatabase()
        {
            _settings.Clear();
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
            T value;

            // If the key exists, retrieve the value.
            if (_settings.Contains(key))
            {
                value = (T)_settings[key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            _settings.Save();
        }
    }
}
