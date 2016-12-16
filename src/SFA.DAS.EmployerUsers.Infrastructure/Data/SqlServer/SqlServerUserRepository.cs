using System;
using System.Linq;
using System.Threading.Tasks;
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
            var user = await QuerySingle<User>("GetUserById @id", new { id });
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
            var user = await QuerySingle<User>("GetUserByEmail @emailAddress", new { emailAddress });
            if (user == null)
            {
                return null;
            }

            user.SecurityCodes = await GetUserSecurityCodes(user);
            user.PasswordHistory = await GetUserPasswordHistory(user);
            return user;
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



        private async Task<SecurityCode[]> GetUserSecurityCodes(User user)
        {
            return await Query<SecurityCode>("GetUserSecurityCodes @Id", user);
        }
        private async Task UpdateUserSecurityCodes(User user, IUnitOfWork unitOfWork)
        {
            var existingCodes = (await GetUserSecurityCodes(user)).ToList();

            foreach (var code in user.SecurityCodes)
            {
                if (!existingCodes.Remove(code))
                {
                    await unitOfWork.Execute("CreateUserSecurityCode @Code, @UserId, @CodeType, @ExpiryTime, @ReturnUrl, @PendingValue",
                        new { code.Code, UserId = user.Id, code.CodeType, code.ExpiryTime, code.ReturnUrl, code.PendingValue });
                }
            }

            foreach (var code in existingCodes)
            {
                await unitOfWork.Execute("DeleteUserSecurityCode @Code",code);
            }
        }

        private async Task<HistoricalPassword[]> GetUserPasswordHistory(User user)
        {
            return await Query<HistoricalPassword>("GetUserPasswordHistory @Id", user);
        }
        private async Task UpdateUserPasswordHistory(User user, IUnitOfWork unitOfWork)
        {
            var existingHistory = (await GetUserPasswordHistory(user)).ToList();

            foreach (var historicPassword in user.PasswordHistory)
            {
                if (!existingHistory.Remove(historicPassword))
                {
                    await unitOfWork.Execute("CreateHistoricalPassword @UserId, @Password, @Salt, @PasswordProfileId, @DateSet",
                        new { UserId = user.Id, historicPassword.Password, historicPassword.Salt, historicPassword.PasswordProfileId, historicPassword.DateSet });
                }
            }

            foreach (var historicalPassword in existingHistory)
            {
                await unitOfWork.Execute("DeleteHistoricalPassword @UserId, @Password",
                    new { UserId = user.Id, historicalPassword.Password });
            }
        }
    }
}
