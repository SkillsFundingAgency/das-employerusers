using Newtonsoft.Json;
using SFA.DAS.EmployerUsers.Api.Client;

namespace SFA.DAS.EmployerUsers.Support.Web.Configuration
{
    public class EmployerUsersApiConfiguration : IEmployerUsersApiConfiguration
    {
        [JsonRequired]
        public string ApiBaseUrl { get; set; }

        
        public string ClientId { get; set; }

        
        public string ClientSecret { get; set; }

        [JsonRequired]
        public string IdentifierUri { get; set; }

        
        public string Tenant { get; set; }

        
        public string ClientCertificateThumbprint { get; set; }
    }
}