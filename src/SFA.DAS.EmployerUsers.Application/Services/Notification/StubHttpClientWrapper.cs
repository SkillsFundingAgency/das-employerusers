using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class StubHttpClientWrapper : IHttpClientWrapper
    {
        private readonly ILogger _logger;

        public StubHttpClientWrapper(ILogger logger)
        {
            _logger = logger;
        }

        public Task SendMessage<T>(T content)
        {
            var json = JsonConvert.SerializeObject(content, Formatting.Indented);
            var msg = "Send http message\n" + json;
            _logger.Debug(msg);
            return Task.FromResult<object>(null);
        }
    }
}
