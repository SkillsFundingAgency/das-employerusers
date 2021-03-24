using System.Configuration;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class NotificationsApiConfiguration : INotificationsApiClientConfiguration
    {
        public NotificationsApiConfiguration()
        {
            BaseUrl = ConfigurationManager.AppSettings["NotificationsApiBaseUrl"];
            ClientToken = ConfigurationManager.AppSettings["NotificationsApiClientToken"];
        }
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}
