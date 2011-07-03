using System;

namespace derpirc.Data
{
    public interface IUnitOfWork : IDisposable
    {
        //int ExecuteCommand(string commandText, params object[] parameter);
        bool InitializeDatabase();
        void Commit();
    }
}
