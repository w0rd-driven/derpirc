using System;

namespace derpirc.Data
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
