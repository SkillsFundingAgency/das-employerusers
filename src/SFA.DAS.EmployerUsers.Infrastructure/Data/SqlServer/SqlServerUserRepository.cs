using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using NLog;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public class SqlServerUserRepository : SqlServerRepository, IUserRepository
    {
        private readonly ILogger _logger;

        public SqlServerUserRepository(ILogger logger)
            : base("UsersConnectionString")
        {
            _logger = logger;
        }
        
        public async Task<User> GetById(string id)
        {
            using (var connection = await GetOpenConnection())
            {
                var user = await QuerySingle<User>(connection, "GetUserById @id", new { id });
                if (user == null)
                {
                    return null;
                }

                user.SecurityCodes = await GetUserSecurityCodes(connection, user);
                user.PasswordHistory = await GetUserPasswordHistory(connection, user);
                return user;
            }
        }

        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            using (var connection = await GetOpenConnection())
            {
                var user = await QuerySingle<User>(connection, "GetUserByEmail @emailAddress", new {emailAddress});
                if (user == null)
                {
                    return null;
                }

                user.SecurityCodes = await GetUserSecurityCodes(connection, user);
                user.PasswordHistory = await GetUserPasswordHistory(connection, user);
                return user;
            }
        }

        public async Task<User[]> GetUsersWithExpiredRegistrations()
        {
            using (var connection = await GetOpenConnection())
            {
                var users = await Query<User>(connection, "GetUsersWithExpiredRegistrations");

                foreach (var user in users)
                {
                    user.SecurityCodes = await GetUserSecurityCodes(connection, user);
                    user.PasswordHistory = await GetUserPasswordHistory(connection, user);
                }

                return users;
            }
        }

        public async Task<User[]> GetUsers(int pageSize, int pageNumber)
        {
            var users = await Query<User>("GetUsers @pageSize, @offSet", new { pageSize, offset = (pageNumber  * pageSize) - pageSize });
            return users;
        }
        
        public async Task Create(User registerUser)
        {
            try
            {
                using (var unitOfWork = await GetUnitOfWork())
                {
                    await unitOfWork.Execute("CreateUser @Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked",
                         registerUser);
                    await UpdateUserSecurityCodes(registerUser, unitOfWork);
                    await UpdateUserPasswordHistory(registerUser, unitOfWork);
                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }


        }
        public async Task Update(User user)
        {
            try
            {
                using (var unitOfWork = await GetUnitOfWork())
                {
                    await unitOfWork.Execute("UpdateUser @Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked",
                    user);
                    await UpdateUserSecurityCodes(user, unitOfWork);
                    await UpdateUserPasswordHistory(user, unitOfWork);
                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }

        }
        public async Task Delete(User user)
        {
            try
            {
                using (var unitOfWork = await GetUnitOfWork())
                {
                    await unitOfWork.Execute("DeleteUser @Id", user);
                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public async Task<int> GetUserCount()
        {
            var recordCount = await QuerySingle<int>("UserCount");
            return recordCount;
        }
        
        public async Task<Users> SearchUsers(string criteria, int pageSize, int pageNumber)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Criteria", criteria, DbType.String);
            parameters.Add("@pageSize", pageSize, DbType.Int32);
            parameters.Add("@offset", (pageNumber * pageSize) - pageSize, DbType.Int32);
            parameters.Add("@totalRecords", 0, DbType.Int32, ParameterDirection.Output);
            var users = await Query<User>("SearchUsers @Criteria, @pageSize, @offset, @totalRecords OUTPUT", parameters);

            return new Users { UserCount = parameters.Get<int>("@totalRecords"), UserList = users };
        }

        private async Task<SecurityCode[]> GetUserSecurityCodes(SqlConnection connection, User user)
        {
            return await Query<SecurityCode>(connection, "GetUserSecurityCodes @Id", user);
        }

        private async Task UpdateUserSecurityCodes(User user, IUnitOfWork unitOfWork)
        {
            await unitOfWork.Execute("DeleteAllUserSecurityCodes @UserId", new { UserId = user.Id });
            foreach (var code in user.SecurityCodes)
            {
                await unitOfWork.Execute("CreateUserSecurityCode @Code, @UserId, @CodeType, @ExpiryTime, @ReturnUrl, @PendingValue",
                    new { code.Code, UserId = user.Id, code.CodeType, code.ExpiryTime, code.ReturnUrl, code.PendingValue });
            }
        }

        private async Task<HistoricalPassword[]> GetUserPasswordHistory(SqlConnection connection, User user)
        {
            return await Query<HistoricalPassword>(connection, "GetUserPasswordHistory @Id", user);
        }

        private async Task UpdateUserPasswordHistory(User user, IUnitOfWork unitOfWork)
        {
            await unitOfWork.Execute("DeleteUserPasswordHistory @UserId", new { UserId = user.Id });
            foreach (var historicPassword in user.PasswordHistory)
            {
                await unitOfWork.Execute("CreateHistoricalPassword @UserId, @Password, @Salt, @PasswordProfileId, @DateSet",
                    new { UserId = user.Id, historicPassword.Password, historicPassword.Salt, historicPassword.PasswordProfileId, historicPassword.DateSet });
            }
        }
    }
}
