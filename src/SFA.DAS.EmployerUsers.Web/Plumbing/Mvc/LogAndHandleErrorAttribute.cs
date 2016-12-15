using System.Web.Mvc;
using NLog;

namespace SFA.DAS.EmployerUsers.Web.Plumbing.Mvc
{
    public class LogAndHandleErrorAttribute : HandleErrorAttribute
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public override void OnException(ExceptionContext filterContext)
        {
            var error = filterContext.Exception;
            Logger.Error(error, "Unhandled exception - " + error.Message);

            base.OnException(filterContext);
        }
    }
}