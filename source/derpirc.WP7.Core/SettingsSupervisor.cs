using System;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;

namespace derpirc.Core
{
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

        private void ConfigureRunningState()
        {
            var defaultSession = _unitOfWork.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
            var configSession = SettingsUnitOfWork.Default.Session;
            if (defaultSession == null)
                defaultSession = CreateSession();
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
            _unitOfWork.Commit();
        }

        #region Factory methods

        private Session CreateSession()
        {
            var result = new Session();
            result.Name = "default";
            _unitOfWork.Sessions.Add(result);
            _unitOfWork.Commit();
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

        public void Commit()
        {
            SettingsUnitOfWork.Default.Commit();
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
