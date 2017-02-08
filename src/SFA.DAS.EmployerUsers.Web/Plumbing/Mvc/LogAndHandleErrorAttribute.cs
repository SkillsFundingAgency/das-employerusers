using System.Web.Mvc;
using NLog;
using Microsoft.ApplicationInsights;

namespace SFA.DAS.EmployerUsers.Web.Plumbing.Mvc
{
    public class LogAndHandleErrorAttribute : HandleErrorAttribute
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public override void OnException(ExceptionContext filterContext)
        {
            var error = filterContext.Exception;
            var controller = GetContollerName(filterContext);
            var action = GetActionName(filterContext);
            var verb = GetHttpVerb(filterContext);

            Logger.Error(error, $"Unhandled exception from {controller}.{action}({verb}) - {error.Message}");

            var ai = new TelemetryClient();
            ai.TrackException(filterContext.Exception);

            base.OnException(filterContext);
        }


        private string GetContollerName(ExceptionContext filterContext)
        {
            try
            {
                return filterContext?.RouteData?.Values["controller"]?.ToString() ?? "UNKNOWN";
            }
            catch
            {
                return "UNKNOWN";
            }
        }
        private string GetActionName(ExceptionContext filterContext)
        {
            try
            {
                return filterContext?.RouteData?.Values["action"]?.ToString() ?? "UNKNOWN";
            }
            catch
            {
                return "UNKNOWN";
            }
        }
        private string GetHttpVerb(ExceptionContext filterContext)
        {
            try
            {
                return filterContext?.RequestContext?.HttpContext?.Request?.HttpMethod ?? "UNKNOWN";
            }
            catch
            {
                return "UNKNOWN";
            }
        }
    }
}