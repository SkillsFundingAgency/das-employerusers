using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;
        private bool _pending;

        public UnitOfWork(IDbConnection connection)
        {
            _connection = connection as SqlConnection;
        }

        public async Task BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_connection != null)
            {
                await OpenConnection();
                _transaction = _connection.BeginTransaction(isolationLevel);
                _pending = true;
            }
        }
        
        public void CommitChanges()
        {
            if (_transaction != null && _pending)
            {
                _transaction.Commit();
                _pending = false;
            }
        }
        
        public void RollbackChanges()
        {
            if (_transaction != null && _pending)
            {
                _transaction.Rollback();
                _pending = false;
            }
        }

        public async Task<T[]> Query<T>(string command, object param = null)
        {
            return await Query<T>(_transaction?.Connection ?? _connection, command, param);
        }

        public async Task<T> QuerySingle<T>(string command, object param = null)
        {
            return (await Query<T>(_transaction?.Connection ?? _connection, command, param)).SingleOrDefault();
        }

        public async Task Execute(string command, object param = null)
        {
            await Execute(_transaction?.Connection ?? _connection, command, param);
        }

        public void Dispose()
        {
            RollbackChanges();
            
            _transaction?.Dispose();
            _connection?.Dispose();
        }

        private async Task<T[]> Query<T>(SqlConnection connection, string command, object param = null)
        {
            await OpenConnection();
            return (await connection.QueryAsync<T>(command, param)).ToArray();
        }

        private async Task Execute(SqlConnection connection, string command, object param = null)
        {
            await OpenConnection();
            await connection.ExecuteAsync(command, param, _transaction);
        }

        private async Task OpenConnection()
        {
            if (!_connection.State.HasFlag(ConnectionState.Open))
            {
                await _connection.OpenAsync();
            }
        }
    }
}