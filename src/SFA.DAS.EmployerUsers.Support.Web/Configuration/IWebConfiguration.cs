using SFA.DAS.Support.Shared.SiteConnection;

namespace SFA.DAS.EmployerUsers.Support.Web.Configuration
{
    public interface IWebConfiguration
    {
        EmployerUsersApiConfiguration EmployerUsersApi { get; set; }
        SiteValidatorSettings SiteValidator { get; set; }
    }
}