using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public abstract class FileSystemRepository
    {
        protected readonly string _directory;

        protected FileSystemRepository(string appDataFolderName)
        {
            var appData = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            _directory = Path.Combine(appData, appDataFolderName);
        }

        protected string[] GetDataFiles()
        {
            return Directory.GetFiles(_directory, "*.json");
        }

        protected async Task<T> ReadFileById<T>(string id)
        {
            var path = Path.Combine(_directory, id + ".json");
            if (!File.Exists(path))
            {
                return default(T);
            }

            return await ReadFile<T>(path);
        }
        protected async Task<T> ReadFile<T>(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync();
                reader.Close();

                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        protected async Task CreateFile<T>(T item, string id)
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            var path = Path.Combine(_directory, id + ".json");
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                var json = JsonConvert.SerializeObject(item, Formatting.Indented);
                await writer.WriteAsync(json);
                await writer.FlushAsync();
                writer.Close();
            }
        }
    }
}
