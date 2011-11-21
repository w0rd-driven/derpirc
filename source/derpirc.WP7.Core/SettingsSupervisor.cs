using System;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;

namespace derpirc.Core
{
    /// <summary>
    /// Responsible for converting settings session to running state
    /// </summary>
    public class SettingsSupervisor : IDisposable
    {
        private bool _isDisposed;

        private SettingsSync _settingsToSync;
        private Action<SettingsSync> _completedAction;

        public SettingsSupervisor()
        {
            this._settingsToSync = new SettingsSync();
        }

        /// <summary>
        /// Workhorse responsible for synchronizing configured and running sessions
        /// </summary>
        private void ConfigureRunningState()
        {
            Session newSession = null;
            using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString()))
            {
                var defaultSession = unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
                var configNetworks = SettingsUnitOfWork.Default.Networks;
                if (defaultSession == null)
                {
                    defaultSession = CreateSession();
                    newSession = defaultSession;
                }
                foreach (var network in configNetworks)
                {
                    var foundNetwork = defaultSession.Networks.Where(x => x.Name == network.Name).FirstOrDefault();
                    if (foundNetwork == null)
                        foundNetwork = CreateNetwork(defaultSession, network);
                    else
                    {
                        foundNetwork.HostName = network.HostName;
                        foundNetwork.Ports = network.Ports;
                        foundNetwork.Password = network.Password;
                    }
                    foreach (var favorite in network.Favorites)
                    {
                        var foundFavorite = foundNetwork.Favorites.Where(x => x.Name == favorite.Name).FirstOrDefault();
                        if (foundNetwork == null)
                            CreateFavorite(foundNetwork, favorite);
                        else
                        {
                            foundFavorite.IsAutoConnect = favorite.IsAutoConnect;
                            foundFavorite.Password = favorite.Password;
                        }
                    }
                }
                if (newSession != null)
                    unitOfWork.Sessions.Add(defaultSession);
                unitOfWork.Commit();
            }
            PurgeOrphans();
        }

        /// <summary>
        /// Remove any stale channel or network logs
        /// </summary>
        private void PurgeOrphans()
        {
            var isPurged = false;

            using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString()))
            {
                var defaultSession = unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
                var configNetworks = SettingsUnitOfWork.Default.Networks;
                foreach (var network in defaultSession.Networks)
                {
                    var configNetwork = configNetworks.Where(x => x.Name == network.Name).FirstOrDefault();
                    if (configNetwork == null)
                        _settingsToSync.OldNetworks.Add(network.Name);
                    else
                    {
                        foreach (var favorite in network.Favorites)
                        {
                            var configFavorite = configNetwork.Favorites.FirstOrDefault(x => x.Name == favorite.Name);
                            if (configFavorite == null)
                                _settingsToSync.OldFavorites.Add(new Tuple<string, string>(network.Name, favorite.Name));
                        }
                    }
                    if (_settingsToSync.OldFavorites.Count > 0)
                    {
                        foreach (var favorite in _settingsToSync.OldFavorites)
                        {
                            var favoritesToDelete = network.Favorites.Where(x => x.Name == favorite.Item2);
                            foreach (var item in favoritesToDelete)
                            {
                                unitOfWork.Favorites.Remove(item);
                            }
                            var channelsToDelete = network.Channels.Where(x => x.Name == favorite.Item2);
                            foreach (var item in channelsToDelete)
                            {
                                unitOfWork.Channels.Remove(item);
                            }
                            var mentionsToDelete = network.Mentions.Where(x => x.ChannelName == favorite.Item2);
                            foreach (var item in mentionsToDelete)
                            {
                                unitOfWork.Mentions.Remove(item);
                            }
                            isPurged = true;
                        }
                    }
                }

                if (_settingsToSync.OldNetworks.Count > 0)
                {
                    foreach (var network in _settingsToSync.OldNetworks)
                    {
                        var recordsToDelete = defaultSession.Networks.Where(x => x.Name == network);
                        foreach (var item in recordsToDelete)
                        {
                            unitOfWork.Networks.Remove(item);
                        }
                        isPurged = true;
                    }
                }
                if (isPurged)
                    unitOfWork.Commit();
            }
            if (_completedAction != null)
            {
                _completedAction(_settingsToSync);
                _completedAction = null;
            }
        }

        #region Factory methods

        private Session CreateSession()
        {
            var result = new Session();
            result.Name = "default";
            return result;
        }

        private Network CreateNetwork(Session parentRecord, Data.Models.Settings.Network setting)
        {
            var result = new Network();
            result.Name = setting.Name;
            result.DisplayName = setting.DisplayName;
            result.HostName = setting.HostName;
            // Create this once since it can't be null but we should never update this directly.
            result.ConnectedHostName = setting.HostName;
            result.Ports = setting.Ports;
            result.Password = setting.Password;
            foreach (var favorite in setting.Favorites)
            {
                CreateFavorite(result, favorite);
            }
            parentRecord.Networks.Add(result);

            _settingsToSync.NewNetworks.Add(setting.Name);

            return result;
        }

        private void CreateFavorite(Network parentRecord, Data.Models.Settings.Favorite setting)
        {
            var newRecord = new Favorite();
            newRecord.Name = setting.Name;
            newRecord.IsAutoConnect = setting.IsAutoConnect;
            newRecord.Password = setting.Password;
            parentRecord.Favorites.Add(newRecord);
        }

        #endregion

        /// <summary>
        /// Calls private ConfigureRunningState() and PurgeOrphans() methods
        /// </summary>
        public void Commit(Action<SettingsSync> completedAction)
        {
            this._completedAction = completedAction;
            ConfigureRunningState();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                }
            }
            this._isDisposed = true;
        }
    }
}
