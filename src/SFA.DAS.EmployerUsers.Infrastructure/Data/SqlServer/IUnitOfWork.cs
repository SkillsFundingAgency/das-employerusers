using System;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void CommitChanges();
        void RollbackChanges();
        Task<T[]> Query<T>(string command, object param = null);
        Task<T> QuerySingle<T>(string command, object param = null);
        Task Execute(string command, object param = null);
    }
}