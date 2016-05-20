namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class EmployerUsersConfiguration
    {
        public IdentityServerConfiguration IdentityServer { get; set; }
        public DataStorageConfiguration DataStorage { get; set; }
    }

    public class DataStorageConfiguration
    {
        public string DocumentDbUri { get; set; }
        public string DocumentDbAccessToken { get; set; }
    }

    public class IdentityServerConfiguration
    {
        public string CertificateStore { get; set; }
        public string CertificateThumbprint { get; set; }
    }
}
