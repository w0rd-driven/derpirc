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
        #region Properties

        #endregion

        private bool _isDisposed;

        private DataUnitOfWork _unitOfWork;

        public SettingsSupervisor(DataUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Workhorse responsible for synchronizing configured and running sessions
        /// </summary>
        private void ConfigureRunningState()
        {
            Session newSession = null;
            var defaultSession = _unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
            var configSession = SettingsUnitOfWork.Default.Session;
            if (defaultSession == null)
            {
                defaultSession = CreateSession();
                newSession = defaultSession;
            }
            foreach (var network in configSession.Networks)
            {
                var foundNetwork = defaultSession.Networks.Where(x => x.Name == network.Name).FirstOrDefault();
                if (foundNetwork == null)
                    foundNetwork = CreateNetwork(defaultSession, network);
                else
                {
                    foundNetwork.Name = network.Name;
                    foundNetwork.DisplayName = network.DisplayName;
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
                        foundFavorite.Name = favorite.Name;
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
            List<int> networksToSmash = new List<int>();
            List<int> favoritesToSmash = new List<int>();

            var defaultSession = _unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
            var configSession = SettingsUnitOfWork.Default.Session;
            for (int index = 0; index < defaultSession.Networks.Count - 1; index++)
            {
                var record = defaultSession.Networks[index];
                var foundNetwork = configSession.Networks.Where(x => x.Name == record.Name).FirstOrDefault();
                if (foundNetwork == null)
                    networksToSmash.Add(index);
                else
                {
                    for (int indexFavorite = 0; indexFavorite < foundNetwork.Favorites.Count - 1; indexFavorite++)
                    {
                        var favoriteRecord = record.Favorites[index];
                        var foundFavorite = foundNetwork.Favorites.Where(x => x.Name == record.Name).FirstOrDefault();
                        if (foundNetwork == null)
                            favoritesToSmash.Add(index);
                    }
                }
                if (favoritesToSmash.Count > 0)
                {
                    for (int item = favoritesToSmash.Count - 1; item >= 0; --item)
                    {
                        foundNetwork.Favorites.RemoveAt(item);
                    }
                }
                favoritesToSmash.Clear();
            }
            if (networksToSmash.Count > 0)
            {
                for (int item = networksToSmash.Count - 1; item >= 0; --item)
                {
                    defaultSession.Networks.RemoveAt(item);
                }
            }
            _unitOfWork.Commit();
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
        public void Commit()
        {
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
