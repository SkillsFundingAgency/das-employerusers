using Newtonsoft.Json;
using SFA.DAS.Support.Shared.SiteConnection;

namespace SFA.DAS.EmployerUsers.Support.Web.Configuration
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public EmployerUsersApiConfiguration EmployerUsersApi { get; set; }

        [JsonRequired]
        public AccountApiConfiguration AccountApi { get; set; }

        
        [JsonRequired]
        public SiteValidatorSettings SiteValidator { get; set; }
    }
}