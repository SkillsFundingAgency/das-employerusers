using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public class SqlServerUserRepository : SqlServerRepository, IUserRepository
    {
        public SqlServerUserRepository()
            : base("UsersConnectionString")
        {
        }

        public async Task<User> GetById(string id)
        {
            var user = await QuerySingle<User>("GetUserById @id", new { id });
            if (user == null)
            {
                return null;
            }

            user.SecurityCodes = await GetUserSecurityCodes(user);
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
            return user;
        }

        public async Task Create(User registerUser)
        {
            await Execute("CreateUser @Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked, @PendingEmail",
                registerUser);
            await UpdateUserSecurityCodes(registerUser);
        }

        public async Task Update(User user)
        {
            await Execute("UpdateUser @Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked, @PendingEmail",
                user);
            await UpdateUserSecurityCodes(user);
        }



        private async Task<SecurityCode[]> GetUserSecurityCodes(User user)
        {
            return await Query<SecurityCode>("GetUserSecurityCodes @Id", user);
        }
        private async Task UpdateUserSecurityCodes(User user)
        {
            var existingCodes = (await GetUserSecurityCodes(user)).ToList();

            foreach (var code in user.SecurityCodes)
            {
                if (!existingCodes.Remove(code))
                {
                    await Execute("CreateUserSecurityCode @Code, @UserId, @CodeType, @ExpiryTime, @ReturnUrl",
                        new { code.Code, UserId = user.Id, code.CodeType, code.ExpiryTime, code.ReturnUrl });
                }
            }

            foreach (var code in existingCodes)
            {
                await Execute("DeleteUserSecurityCode @Code", code);
            }
        }
    }
}
