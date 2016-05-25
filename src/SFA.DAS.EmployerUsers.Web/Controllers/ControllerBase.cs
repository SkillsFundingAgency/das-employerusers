using System;
using System.Web.Mvc;
using NLog;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class ControllerBase : Controller
    {
        protected readonly Logger Logger;

        public ControllerBase()
        {
            Logger = LogManager.GetLogger(GetType().FullName);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                if (filterContext.Exception is AggregateException)
                {
                    ((AggregateException) filterContext.Exception).UnpackAndLog(Logger);
                }
                else
                {
                    Logger.Error(filterContext.Exception, filterContext.Exception.Message);
                }
            }
            base.OnException(filterContext);
        }
    }
}