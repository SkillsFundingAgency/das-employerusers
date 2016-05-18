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

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync();
                reader.Close();

                return JsonConvert.DeserializeObject<User>(json);
            }
        }

        public async Task Create(User registerUser)
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            registerUser.Id = Guid.NewGuid().ToString();

            var path = Path.Combine(_directory, registerUser.Id + ".json");
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                var json = JsonConvert.SerializeObject(registerUser);
                await writer.WriteAsync(json);
                await writer.FlushAsync();
                writer.Close();
            }
        }
    }
}
