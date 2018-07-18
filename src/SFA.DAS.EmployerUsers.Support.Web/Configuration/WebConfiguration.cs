using System;
using Newtonsoft.Json;
using SFA.DAS.Support.Shared.SiteConnection;
using StructureMap;

namespace SFA.DAS.EmployerUsers.Support.Web.Configuration
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public EmployerUsersApiConfiguration EmployerUsersApi { get; set; }

        [JsonRequired]
        public AccountApiConfiguration AccountApi { get; set; }

        [JsonRequired] public ChallengeSettings Challenge { get; set; }
        [JsonRequired]
        public SiteValidatorSettings SiteValidator { get; set; }


        [JsonRequired] public SiteConnectorSettings SiteConnector { get; set; }

        [JsonRequired] public SiteSettings Site { get; set; }
    }
}