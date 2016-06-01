using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class StubHttpClientWrapper : IHttpClientWrapper
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public Task SendMessage(Dictionary<string, string> messageProperties)
        {
            var msg = "Send http message\n" + messageProperties.Select(kvp => $"{kvp.Key} = '{kvp.Value}'")
                .Aggregate((x, y) => $"{x}\n{y}");
            Logger.Debug(msg);
            return Task.FromResult<object>(null);
        }
    }
}
