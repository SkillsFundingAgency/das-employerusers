using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.Authentication;
using SFA.DAS.Support.Shared.SiteConnection;
using SFA.DAS.Web.Policy;

namespace SFA.DAS.EmployerUsers.Support.Web
{
    [ExcludeFromCodeCoverage]
    public class MvcApplication : HttpApplication
    {
        //public static Collection<DelegatingHandler> ConfigurationMessageHandlers;
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            var ioc = DependencyResolver.Current;
            var logger = ioc.GetService<ILog>();
            logger.Info("Starting Web Role");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var siteValidatorSettings = ioc.GetService<ISiteValidatorSettings>();

            GlobalConfiguration.Configuration.MessageHandlers.Add(new TokenValidationHandler(siteValidatorSettings, logger));
            GlobalFilters.Filters.Add(new TokenValidationFilter(siteValidatorSettings, logger));
            logger.Info("Web role started");
        }
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {

            if (HttpContext.Current == null) return;
            new HttpContextPolicyProvider(
                new List<IHttpContextPolicy>()
                {
                    new ResponseHeaderRestrictionPolicy()
                }
            ).Apply(new HttpContextWrapper(HttpContext.Current), PolicyConcern.HttpResponse);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();
            var logger = DependencyResolver.Current.GetService<ILog>();
            logger.Error(ex, "App_Error");
        }
    }
}