using System;

namespace derpirc.Data
{
    public interface IUnitOfWork : IDisposable
    {
        bool InitializeDatabase(bool wipe);
        void Commit();
    }
}
