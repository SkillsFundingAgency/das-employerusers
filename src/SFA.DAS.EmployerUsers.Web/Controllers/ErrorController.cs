using System.Web.Mvc;
using NLog;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public ActionResult General()
        {
            var lastError = Server.GetLastError();
            if (lastError != null)
            {
                Logger.Error(lastError, "Unhandled exception - " + lastError.Message);
            }
            return View("~/Views/Shared/Error.cshtml");
        }
        public ActionResult NotFound(string path = null)
        {
            return View();
        }
    }
}
