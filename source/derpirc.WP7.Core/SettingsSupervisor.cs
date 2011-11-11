using System;
using System.Collections.Generic;
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

        private DataUnitOfWork _unitOfWork;
        private SettingsSync _settingsToSync;
        private Action<SettingsSync> _completedAction;

        public SettingsSupervisor(DataUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._settingsToSync = new SettingsSync();
        }

        /// <summary>
        /// Workhorse responsible for synchronizing configured and running sessions
        /// </summary>
        private void ConfigureRunningState()
        {
            Session newSession = null;
            var defaultSession = _unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
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
                _unitOfWork.Sessions.Add(defaultSession);
            _unitOfWork.Commit();
            PurgeOrphans();
        }

        /// <summary>
        /// Remove any stale channel or network logs
        /// </summary>
        private void PurgeOrphans()
        {
            var isPurged = false;
            List<string> favoritesToRemove = new List<string>();

            var defaultSession = _unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
            var configNetworks = SettingsUnitOfWork.Default.Networks;
            foreach (var network in defaultSession.Networks)
            {
                var configNetwork = configNetworks.Where(x => x.Name == network.Name).FirstOrDefault();
                if (configNetwork == null)
                    _settingsToSync.OldItems.Add(network.Name);
                else
                {
                    foreach (var favorite in network.Favorites)
                    {
                        var configFavorite = configNetwork.Favorites.FirstOrDefault(x => x.Name == favorite.Name);
                        if (configFavorite == null)
                            favoritesToRemove.Add(favorite.Name);
                    }
                }
                if (favoritesToRemove.Count > 0)
                {
                    foreach (var favorite in favoritesToRemove)
                    {
                        var favoritesToDelete = network.Favorites.Where(x => x.Name == favorite);
                        foreach (var item in favoritesToDelete)
                        {
                            _unitOfWork.Favorites.Remove(item);
                        }
                        // TODO: Bubble up to UI somehow...
                        var channelsToDelete = network.Channels.Where(x => x.Name == favorite);
                        foreach (var item in channelsToDelete)
                        {
                            _unitOfWork.Channels.Remove(item);
                        }
                        var mentionsToDelete = network.Mentions.Where(x => x.ChannelName == favorite);
                        foreach (var item in mentionsToDelete)
                        {
                            _unitOfWork.Mentions.Remove(item);
                        }
                        isPurged = true;
                    }
                }
                favoritesToRemove.Clear();
            }

            if (_settingsToSync.OldItems.Count > 0)
            {
                foreach (var network in _settingsToSync.OldItems)
                {
                    // TODO: Bubble up to UI somehow...
                    var recordsToDelete = defaultSession.Networks.Where(x => x.Name == network);
                    foreach (var item in recordsToDelete)
                    {
                        _unitOfWork.Networks.Remove(item);
                    }
                    isPurged = true;
                }
            }
            if (isPurged)
                _unitOfWork.Commit();
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
            result.IsConnected = false;
            foreach (var favorite in setting.Favorites)
            {
                CreateFavorite(result, favorite);
            }
            parentRecord.Networks.Add(result);

            _settingsToSync.NewItems.Add(setting.Name);

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
