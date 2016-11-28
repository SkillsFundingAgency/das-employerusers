using Microsoft.Azure;
using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class NotificationsApiConfiguration : INotificationsApiClientConfiguration
    {
        public NotificationsApiConfiguration()
        {
            BaseUrl = CloudConfigurationManager.GetSetting("NotificationsApiBaseUrl");
            ClientToken = CloudConfigurationManager.GetSetting("NotificationsApiClientToken");
        }
        public string BaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}
