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
                        var isUpdated = false;
                        if (foundNetwork.HostName != null &&
                            !foundNetwork.HostName.Equals(network.HostName, StringComparison.OrdinalIgnoreCase))
                        {
                            foundNetwork.HostName = network.HostName;
                            isUpdated = true;
                        }
                        if (foundNetwork.Ports != null &&
                            !foundNetwork.Ports.Equals(network.Ports, StringComparison.OrdinalIgnoreCase))
                        {
                            foundNetwork.Ports = network.Ports;
                            isUpdated = true;
                        }
                        if (foundNetwork.Password != null &&
                            !foundNetwork.Password.Equals(network.Password, StringComparison.OrdinalIgnoreCase))
                        {
                            foundNetwork.Password = network.Password;
                            isUpdated = true;
                        }
                        if (isUpdated)
                            _settingsToSync.Networks.Add(new Tuple<Network, ChangeType>(foundNetwork, ChangeType.Update));
                    }
                    foreach (var favorite in network.Favorites)
                    {
                        var foundFavorite = foundNetwork.Favorites.Where(x => x.Name == favorite.Name).FirstOrDefault();
                        if (foundFavorite == null)
                            CreateFavorite(foundNetwork, favorite);
                        else
                        {
                            var isUpdated = false;
                            if (foundFavorite.IsAutoConnect != favorite.IsAutoConnect)
                            {
                                foundFavorite.IsAutoConnect = favorite.IsAutoConnect;
                                isUpdated = true;
                            }
                            if (foundFavorite.Password != null &&
                                !foundFavorite.Password.Equals(favorite.Password, StringComparison.OrdinalIgnoreCase))
                            {
                                foundFavorite.Password = favorite.Password;
                                isUpdated = true;
                            }
                            if (isUpdated)
                                _settingsToSync.Favorites.Add(new Tuple<Favorite, ChangeType>(foundFavorite, ChangeType.Update));
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
                        _settingsToSync.Networks.Add(new Tuple<Network, ChangeType>(network, ChangeType.Delete));
                    else
                    {
                        foreach (var favorite in network.Favorites)
                        {
                            var configFavorite = configNetwork.Favorites.FirstOrDefault(x => x.Name == favorite.Name);
                            if (configFavorite == null)
                                _settingsToSync.Favorites.Add(new Tuple<Favorite, ChangeType>(favorite, ChangeType.Delete));
                        }
                    }
                    if (_settingsToSync.Favorites.Count(x => x.Item2 == ChangeType.Delete) > 0)
                    {
                        foreach (var favorite in _settingsToSync.Favorites)
                        {
                            // Remove Favorites
                            var favoritesToDelete = network.Favorites.Where(x => x.Name == favorite.Item1.Name);
                            foreach (var item in favoritesToDelete)
                            {
                                unitOfWork.Favorites.Remove(item);
                                isPurged = true;
                            }
                            // Remove Channels
                            var channelsToDelete = network.Channels.Where(x => x.Name == favorite.Item1.Name);
                            foreach (var item in channelsToDelete)
                            {
                                unitOfWork.Channels.Remove(item);
                                isPurged = true;
                            }
                            // Remove Mentions
                            var mentionsToDelete = network.Mentions.Where(x => x.ChannelName == favorite.Item1.Name);
                            foreach (var item in mentionsToDelete)
                            {
                                unitOfWork.Mentions.Remove(item);
                                isPurged = true;
                            }
                        }
                    }
                }

                if (_settingsToSync.Networks.Count(x => x.Item2 == ChangeType.Delete) > 0)
                {
                    foreach (var network in _settingsToSync.Networks)
                    {
                        var recordsToDelete = defaultSession.Networks.Where(x => x.Name == network.Item1.Name);
                        foreach (var item in recordsToDelete)
                        {
                            unitOfWork.Networks.Remove(item);
                            isPurged = true;
                        }
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

            _settingsToSync.Networks.Add(new Tuple<Network, ChangeType>(result, ChangeType.Insert));

            return result;
        }

        private void CreateFavorite(Network parentRecord, Data.Models.Settings.Favorite setting)
        {
            var result = new Favorite();
            result.Name = setting.Name;
            result.IsAutoConnect = setting.IsAutoConnect;
            result.Password = setting.Password;
            parentRecord.Favorites.Add(result);

            // We don'e actually care for Favorite inserts. They get handled by new networks/updates
            _settingsToSync.Favorites.Add(new Tuple<Favorite, ChangeType>(result, ChangeType.Insert));
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
