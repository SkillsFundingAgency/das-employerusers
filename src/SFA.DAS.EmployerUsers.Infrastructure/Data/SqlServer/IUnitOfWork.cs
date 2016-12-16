using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public interface IUnitOfWork : IDisposable
    {
        void CommitChanges();
        void RollbackChanges();
        Task Execute(string command, object param = null);
    }
}