using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Web.Policy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SFA.DAS.EmployerUsers.Support.Web
{
    [ExcludeFromCodeCoverage]
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];

            MvcHandler.DisableMvcResponseHeader = true;
            var logger = DependencyResolver.Current.GetService<ILog>();
            logger.Info("Starting Web Role");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            logger.Info("Web role started");
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            if (HttpContext.Current == null) return;
            new HttpContextPolicyProvider(
                new List<IHttpContextPolicy>
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