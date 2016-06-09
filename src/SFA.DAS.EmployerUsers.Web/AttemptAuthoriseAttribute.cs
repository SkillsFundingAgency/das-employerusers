using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web
{
    public class AttemptAuthoriseAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            filterContext.Result = null;
        }
    }
}