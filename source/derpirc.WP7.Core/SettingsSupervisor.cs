using System;
using derpirc.Data;

namespace derpirc.Core
{
    public class SettingsSupervisor : IDisposable
    {
        #region Properties

        #endregion

        private bool _isDisposed;

        private DataUnitOfWork _unitOfWork;
        private SettingsUnitOfWork _unitOfWorkSettings;

        public SettingsSupervisor(DataUnitOfWork unitOfWork, SettingsUnitOfWork unitOfWorkSettings)
        {
            this._unitOfWork = unitOfWork;
            this._unitOfWorkSettings = unitOfWorkSettings;
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
