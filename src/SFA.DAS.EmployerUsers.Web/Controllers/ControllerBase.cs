using System;
using System.Net;
using System.Web.Mvc;
using NLog;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Web.Models;

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

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var orchestratorResponse = model as OrchestratorResponse;

            if (orchestratorResponse == null) return base.View(viewName, masterName, model);

            var invalidRequestException = orchestratorResponse.Exception as InvalidRequestException;

            if (invalidRequestException != null)
            {
                foreach (var errorMessageItem in invalidRequestException.ErrorMessages)
                {
                    ModelState.AddModelError(errorMessageItem.Key, errorMessageItem.Value);
                }
            }

            if (orchestratorResponse.Status == HttpStatusCode.OK)
                return base.View(viewName, masterName, orchestratorResponse);

            if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
            {
                return base.View(@"Error", masterName, orchestratorResponse);
            }

            if (orchestratorResponse.Status == HttpStatusCode.BadRequest)
            {
                return base.View(@"BadRequest", masterName, orchestratorResponse);
            }

            return base.View(@"Error", masterName, orchestratorResponse);
        }
    }
}