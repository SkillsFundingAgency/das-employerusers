using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public class FileSystemUserRepository : IUserRepository
    {
        private readonly string _directory;

        public FileSystemUserRepository()
        {
            var appData = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            _directory = Path.Combine(appData, "Users");
        }

        public async Task<User> GetById(string id)
        {
            var path = Path.Combine(_directory, id + ".json");
            if (!File.Exists(path))
            {
                return null;
            }

            return await ReadUser(path);
        }
        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            var userFiles = Directory.GetFiles(_directory, "*.json");

            foreach (var path in userFiles)
            {
                var user = await ReadUser(path);
                if (user.Email.Equals(emailAddress, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }

            return null;
        }
        public async Task Create(User registerUser)
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            registerUser.Id = Guid.NewGuid().ToString();

            var path = GetUserFilePath(registerUser);
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                var json = JsonConvert.SerializeObject(registerUser);
                await writer.WriteAsync(json);
                await writer.FlushAsync();
                writer.Close();
            }
        }
        public async Task Update(User user)
        {
            var path = GetUserFilePath(user);

            File.Delete(path);

            await Create(user);

        }


		private string GetUserFilePath(User registerUser)
        {
            return Path.Combine(_directory, registerUser.Id + ".json");
        }
		private async Task<User> ReadUser(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync();
                reader.Close();

            }
        }
    }
}
