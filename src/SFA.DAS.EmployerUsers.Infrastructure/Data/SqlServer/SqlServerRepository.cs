﻿using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public abstract class SqlServerRepository
    {
        protected SqlServerRepository(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        protected string ConnectionStringName { get; }

        protected async Task<SqlConnection> CreateOpenConnection()
        {
            var connectionString = ConfigurationManager.AppSettings[ConnectionStringName];
            var connection = new SqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
            }
            catch
            {
                connection.Dispose();
                throw;
            }

            return connection;
        }

        protected async Task<UnitOfWork> GetUnitOfWork()
        {
            return new UnitOfWork(await CreateOpenConnection());
        }

        protected async Task<T[]> Query<T>(string command, object param = null)
        {
            using (var connection = await CreateOpenConnection())
            {
                return (await connection.QueryAsync<T>(command, param)).ToArray();
            }
        }

        protected async Task<T[]> Query<T>(SqlConnection connection, string command, object param = null)
        {
            return (await connection.QueryAsync<T>(command, param)).ToArray();
        }

        protected async Task<T> QuerySingle<T>(SqlConnection connection, string command, object param = null)
        {
            return (await Query<T>(connection, command, param)).SingleOrDefault();
        }

        protected async Task<T> QuerySingle<T>(string command, object param = null)
        {
            return (await Query<T>(command, param)).SingleOrDefault();
        }

        protected async Task Execute(string command, object param = null)
        {
            using (var connection = await CreateOpenConnection())
            {
                await connection.ExecuteAsync(command, param);
            }
        }

        protected async Task Execute(string command, IDbTransaction transaction, object param = null)
        {
            await transaction.Connection.ExecuteAsync(command, param, transaction);
        }
    }
}
