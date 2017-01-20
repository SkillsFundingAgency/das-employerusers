using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure;
using NLog;

namespace SFA.DAS.EmployerUsers.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            TelemetryConfiguration.Active.InstrumentationKey = CloudConfigurationManager.GetSetting("InstrumentationKey");

            MvcHandler.DisableMvcResponseHeader = true;
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            LoggingConfig.ConfigureLogging();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application?.Context != null)
            {
                application.Context.Response.Headers.Remove("Server");
            }
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            LogManager.GetCurrentClassLogger().Error(ex, $"Application_Error - {ex.Message}");
        }
    }
}
