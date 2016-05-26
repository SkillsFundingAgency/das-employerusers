using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.WebClientComponents
{
    public class AuthoriseActiveUserAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                return;
            }

            var user = filterContext.HttpContext.User as ClaimsPrincipal;
            var requiresVerification = user?.Claims.FirstOrDefault(c => c.Type == DasClaimTypes.RequiresVerification)?.Value;
            if (string.IsNullOrEmpty(requiresVerification) || requiresVerification.Equals("true", System.StringComparison.OrdinalIgnoreCase))
            {
                var configuration = ConfigurationFactory.Current.Get();
                filterContext.Result = new RedirectResult(configuration.AccountActivationUrl);
            }
        }
    }
}
