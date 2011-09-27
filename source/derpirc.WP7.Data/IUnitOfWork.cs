using System;

namespace derpirc.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void InitializeDatabase(bool wipe);
        void WipeDatabase();
    }
}
