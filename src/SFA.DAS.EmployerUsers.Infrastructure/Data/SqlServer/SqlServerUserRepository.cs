using Dapper;
using NLog;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public class SqlServerUserRepository : IUserRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public SqlServerUserRepository(IUnitOfWork unitOfWork, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<User> GetById(string id)
        {
            var user = await _unitOfWork.QuerySingle<User>("GetUserById @id", new { id });
            if (user == null)
            {
                return null;
            }

            user.SecurityCodes = await GetUserSecurityCodes(user);
            user.PasswordHistory = await GetUserPasswordHistory(user);
            return user;
        }

        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            var user = await _unitOfWork.QuerySingle<User>("GetUserByEmail @emailAddress", new { emailAddress });
            if (user == null)
            {
                return null;
            }

            user.SecurityCodes = await GetUserSecurityCodes(user);
            user.PasswordHistory = await GetUserPasswordHistory(user);
            return user;
        }

        public async Task<User[]> GetUsersWithExpiredRegistrations()
        {
            var users = await _unitOfWork.Query<User>("GetUsersWithExpiredRegistrations");

            foreach (var user in users)
            {
                user.SecurityCodes = await GetUserSecurityCodes(user);
                user.PasswordHistory = await GetUserPasswordHistory(user);
            }

            return users;
        }

        public async Task<User[]> GetUsers(int pageSize, int pageNumber)
        {
            var users = await _unitOfWork.Query<User>("GetUsers @pageSize, @offSet", new { pageSize, offset = (pageNumber  * pageSize) - pageSize });
            return users;
        }
        
        public async Task Create(User registerUser)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                
                await _unitOfWork.Execute("CreateUser @Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked", registerUser);
                await UpdateUserSecurityCodes(registerUser);
                await UpdateUserPasswordHistory(registerUser);
                
                _unitOfWork.CommitChanges();
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
                await _unitOfWork.BeginTransaction();
                
                await _unitOfWork.Execute("UpdateUser @Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked", user);
                await UpdateUserSecurityCodes(user);
                await UpdateUserPasswordHistory(user);
                
                _unitOfWork.CommitChanges();
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
                await _unitOfWork.BeginTransaction();
                
                await _unitOfWork.Execute("DeleteUser @Id", user);
                
                _unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public async Task<int> GetUserCount()
        {
            try
            {
                var recordCount = await _unitOfWork.QuerySingle<int>("UserCount");
                return recordCount;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }
        
        public async Task<Users> SearchUsers(string criteria, int pageSize, int pageNumber)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Criteria", criteria, DbType.String);
            parameters.Add("@pageSize", pageSize, DbType.Int32);
            parameters.Add("@offset", (pageNumber * pageSize) - pageSize, DbType.Int32);
            parameters.Add("@totalRecords", 0, DbType.Int32, ParameterDirection.Output);
            var users = await _unitOfWork.Query<User>("SearchUsers @Criteria, @pageSize, @offset, @totalRecords OUTPUT", parameters);

            return new Users { UserCount = parameters.Get<int>("@totalRecords"), UserList = users };
        }

        public async Task Suspend(User user)
        {
            try
            {
                var now = DateTime.Now;
                await _unitOfWork.BeginTransaction();
                
                await _unitOfWork.Execute("UpdateUserSuspension @Id, @state, @suspendedDate", new { user.Id, state = true, suspendedDate = now });
                
                _unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public async Task Resume(User user)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                
                await _unitOfWork.Execute("UpdateUserSuspension @Id, @state", new { user.Id, state = false });
                
                _unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }
        
        public async Task UpdateWithGovIdentifier(User user)
        {
            try
            {
                await _unitOfWork.BeginTransaction();
                
                await _unitOfWork.Execute("UpdateUserGovUkIdentifier @email, @govUkIdentifier", new { user.Email, user.GovUkIdentifier });
                
                _unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }


        private async Task<SecurityCode[]> GetUserSecurityCodes(User user)
        {
            return await _unitOfWork.Query<SecurityCode>("GetUserSecurityCodes @Id", user);
        }

        private async Task UpdateUserSecurityCodes(User user)
        {
            await _unitOfWork.Execute("DeleteAllUserSecurityCodes @UserId", new { UserId = user.Id });
            foreach (var code in user.SecurityCodes)
            {
                await _unitOfWork.Execute("CreateUserSecurityCode @Code, @UserId, @CodeType, @ExpiryTime, @ReturnUrl, @PendingValue, @FailedAttempts",
                    new { code.Code, UserId = user.Id, code.CodeType, code.ExpiryTime, code.ReturnUrl, code.PendingValue, code.FailedAttempts });
            }
        }

        private async Task<HistoricalPassword[]> GetUserPasswordHistory(User user)
        {
            return await _unitOfWork.Query<HistoricalPassword>("GetUserPasswordHistory @Id", user);
        }

        private async Task UpdateUserPasswordHistory(User user)
        {
            await _unitOfWork.Execute("DeleteUserPasswordHistory @UserId", new { UserId = user.Id });
            foreach (var historicPassword in user.PasswordHistory)
            {
                await _unitOfWork.Execute("CreateHistoricalPassword @UserId, @Password, @Salt, @PasswordProfileId, @DateSet",
                    new { UserId = user.Id, historicPassword.Password, historicPassword.Salt, historicPassword.PasswordProfileId, historicPassword.DateSet });
            }
        }
    }
}
