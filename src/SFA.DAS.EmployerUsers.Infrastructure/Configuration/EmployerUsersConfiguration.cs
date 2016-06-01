namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class EmployerUsersConfiguration
    {
        public IdentityServerConfiguration IdentityServer { get; set; }
        public DataStorageConfiguration DataStorage { get; set; }
        public AccountConfiguration Account { get; set; }
        public EmployerPortalConfiguration EmployerPortalConfiguration { get; set; }
    }
}
