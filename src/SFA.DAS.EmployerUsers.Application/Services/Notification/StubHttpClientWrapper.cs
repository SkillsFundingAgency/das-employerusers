using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class StubHttpClientWrapper : IHttpClientWrapper
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public Task SendMessage<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, Formatting.Indented);
            var msg = "Send http message\n" + json;
            Logger.Debug(msg);
            return Task.FromResult<object>(null);
        }
    }
}
