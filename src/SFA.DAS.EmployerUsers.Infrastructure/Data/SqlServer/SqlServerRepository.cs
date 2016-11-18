using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public abstract class SqlServerRepository
    {
        protected SqlServerRepository(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        protected string ConnectionStringName { get; }

        protected async Task<SqlConnection> GetOpenConnection()
        {
            var connectionString = CloudConfigurationManager.GetSetting(ConnectionStringName);
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

        protected async Task<T[]> Query<T>(string command, object param = null)
        {
            using (var connection = await GetOpenConnection())
            {
                return (await connection.QueryAsync<T>(command, param)).ToArray();
            }
        }
        protected async Task<T> QuerySingle<T>(string command, object param = null)
        {
            return (await Query<T>(command, param)).SingleOrDefault();
        }

        protected async Task Execute(string command, object param = null)
        {
            using (var connection = await GetOpenConnection())
            {
                await connection.ExecuteAsync(command, param);
            }
        }
    }
}
