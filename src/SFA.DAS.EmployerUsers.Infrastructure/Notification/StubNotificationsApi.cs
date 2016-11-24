using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerUsers.Infrastructure.Notification
{
    public class StubNotificationsApi : INotificationsApi
    {
        private readonly string _directory;

        public StubNotificationsApi()
        {
            _directory = Path.Combine((string)AppDomain.CurrentDomain.GetData("DataDirectory"), "Emails");
        }

        public async Task SendEmail(Email email)
        {
            var id = string.IsNullOrEmpty(email.SystemId) ? Guid.NewGuid().ToString() : email.SystemId;
            var path = Path.Combine(_directory, id + ".json");

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(email));
            }
        }
    }
}
