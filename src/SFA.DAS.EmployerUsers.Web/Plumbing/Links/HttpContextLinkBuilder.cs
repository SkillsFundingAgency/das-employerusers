using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.EmployerUsers.Domain.Links;

namespace SFA.DAS.EmployerUsers.Web.Plumbing.Links
{
    public class HttpContextLinkBuilder : ILinkBuilder
    {
        public string GetRegistrationUrl()
        {
            return GetUrlHelper().Action("Register", "Account", null, "https");
        }

        private UrlHelper GetUrlHelper()
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            var requestContext = new RequestContext(httpContext, routeData);
            return new UrlHelper(requestContext);
        }
    }
}