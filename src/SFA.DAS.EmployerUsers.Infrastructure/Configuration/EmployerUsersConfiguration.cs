namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class EmployerUsersConfiguration
    {
        public virtual IdentityServerConfiguration IdentityServer { get; set; }
        public virtual AccountConfiguration Account { get; set; }
        public string Hashstring { get; set; }
        public string AllowedHashstringCharacters { get; set; }
        public string EmployerAccountsBaseUrl { get; set; }
        public string SqlConnectionString { get; set; }

        public virtual AuditConfiguaration AuditApi { get; set; }
    }
}
