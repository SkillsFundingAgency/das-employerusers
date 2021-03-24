using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Audit.Client;
using System.Configuration;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
          
            TelemetryConfiguration.Active.InstrumentationKey = ConfigurationManager.AppSettings["InstrumentationKey"];

            MvcHandler.DisableMvcResponseHeader = true;
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            LoggingConfig.ConfigureLogging();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            AuditMessageFactory.RegisterBuilder(message =>
            {
                var name = typeof(WebApiApplication).Assembly.GetName();

                message.Source = new Audit.Types.Source
                {
                    System = "EMPU",
                    Component = name.Name,
                    Version = name.Version.ToString()
                };
            });
        }
    }
}
