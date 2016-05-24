namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class IdentityServerConfiguration
    {
        public string ApplicationBaseUrl { get; set; }
        public string EmployerPortalUrl { get; set; }
        public string CertificateStore { get; set; }
        public string CertificateThumbprint { get; set; }
    }
}