using System.Configuration;

namespace SFA.DAS.EmployerUsers.EndToEndTests
{
    public class TestSettings
    {
        public TestSettings()
        {
            EmployerUsersUrl = ConfigurationManager.AppSettings["EmployerUsersUrl"];
            UsersConnectionString = ConfigurationManager.AppSettings["UsersConnectionString"];
        }
        public string EmployerUsersUrl { get; }
        public string UsersConnectionString { get; }
    }
}
