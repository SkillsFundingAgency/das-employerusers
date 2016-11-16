using Newtonsoft.Json;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.DocumentDb
{
    public class DocumentDbRelyingParty
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool RequireConsent { get; set; }
        public string ApplicationUrl { get; set; }
        public string LogoutUrl { get; set; }

        internal static DocumentDbRelyingParty FromDomainRelyingParty(Domain.RelyingParty relyingParty)
        {
            return new DocumentDbRelyingParty
            {
                Id = relyingParty.Id,
                Name = relyingParty.Name,
                RequireConsent = relyingParty.RequireConsent,
                ApplicationUrl = relyingParty.ApplicationUrl,
                LogoutUrl = relyingParty.LogoutUrl
            };
        }

        internal Domain.RelyingParty ToDomainRelyingParty()
        {
            return new Domain.RelyingParty
            {
                Id = Id,
                Name = Name,
                RequireConsent = RequireConsent,
                ApplicationUrl = ApplicationUrl,
                LogoutUrl = LogoutUrl
            };
        }
    }
}
