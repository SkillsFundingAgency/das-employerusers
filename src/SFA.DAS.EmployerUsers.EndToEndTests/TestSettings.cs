using System.Configuration;

namespace SFA.DAS.EmployerUsers.EndToEndTests
{
    public class TestSettings
    {
        public TestSettings()
        {
            EmployerUsersUrl = ConfigurationManager.AppSettings["EmployerUsersUrl"];
            UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"];
            ProfilesConnectionString = ConfigurationManager.AppSettings["ProfilesConnectionString"];
        }
        public string EmployerUsersUrl { get; }
        public string UsersConnectionString { get; }
        public string ProfilesConnectionString { get; }
    }
}
