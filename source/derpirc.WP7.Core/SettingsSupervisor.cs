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
        public SettingsSupervisor(DataUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
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
