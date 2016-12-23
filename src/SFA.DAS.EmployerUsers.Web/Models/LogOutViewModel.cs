using IdentityServer3.Core.Models;
using IdentityServer3.Core.ViewModels;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class LogOutViewModel
    {
        public LoggedOutViewModel IdsLogoutModel { get; set; }
        public SignOutMessage SignOutMessage { get; set; }
    }
}