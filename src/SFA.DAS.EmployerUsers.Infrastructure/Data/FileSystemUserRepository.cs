using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public class FileSystemUserRepository : FileSystemRepository, IUserRepository
    {
        public FileSystemUserRepository()
            : base("Users")
        {
        }

        public async Task<User> GetById(string id)
        {
            return await ReadFileById<User>(id);
        }
        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            var userFiles = GetDataFiles();

            foreach (var path in userFiles)
            {
                var user = await ReadFile<User>(path);
                if (user.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }

            return null;
        }
        public async Task Create(User registerUser)
        {
            await CreateFile(registerUser, registerUser.Id);
        }
        public async Task Update(User user)
        {
            var path = Path.Combine(Directory, user.Id + ".json");

            File.Delete(path);

            await Create(user);

        }

        public Task ExpirySecurityCodes(User user, SecurityCodeType codeType)
        {
            throw new NotImplementedException();
        }
    }
}
