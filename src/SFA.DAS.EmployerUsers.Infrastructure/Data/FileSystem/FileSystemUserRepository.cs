using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.FileSystem
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
        public async Task<User[]> GetUsersWithExpiredRegistrations()
        {
            var users = new List<User>();
            var userFiles = GetDataFiles();

            foreach (var path in userFiles)
            {
                var user = await ReadFile<User>(path);
                if (!user.IsActive && !user.SecurityCodes.Any(sc => sc.ExpiryTime >= DateTime.Now))
                {
                    users.Add(user);
                }
            }

            return users.ToArray();
        }

        public async Task<User[]> GetUsers(int pageSize, int pageNumber)
        {
            var users = new List<User>();
            var userFiles = GetDataFiles();

            foreach (var path in userFiles)
            {
                var user = await ReadFile<User>(path);

                users.Add(user);

            }

            return users.ToArray();
        }

        public async Task Create(User registerUser)
        {
            await CreateFile(registerUser, registerUser.Id);
        }
        public async Task Update(User user)
        {
            await Delete(user);
            await Create(user);

        }
        public Task Delete(User user)
        {
            var path = Path.Combine(Directory, user.Id + ".json");

            File.Delete(path);

            return Task.FromResult<object>(null);
        }

        public async Task<int> GetUserCount()
        {
            var users = await this.GetUsers(int.MaxValue, 0);
            return users.Length;
        }

        public async Task<Users> SearchUsers(string criteria, int pageSize, int pageNumber)
        {
            var matchedUsers = new List<User>();
            var userFiles = GetDataFiles();

            foreach (var path in userFiles)
            {
                var user = await ReadFile<User>(path);
                if (UserMatchesSearchCriteria(user, criteria))
                {
                    matchedUsers.Add(user);
                }
            }

            return new Users { UserCount = matchedUsers.Count, UserList = matchedUsers.ToArray() };
        }

        private bool UserMatchesSearchCriteria(User user, string criteria)
        {
            if (user.Email.IndexOf(criteria, StringComparison.OrdinalIgnoreCase) != -1)
            {
                return true;
            }

            if (user.FirstName.IndexOf(criteria, StringComparison.OrdinalIgnoreCase) != -1)
            {
                return true;
            }

            if (user.LastName.IndexOf(criteria, StringComparison.OrdinalIgnoreCase) != -1)
            {
                return true;
            }

            return false;
        }
    }
}
