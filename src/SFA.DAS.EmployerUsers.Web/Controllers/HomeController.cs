using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Debug()
        {
            var envVars = System.Environment.GetEnvironmentVariables();
            return View(envVars);
        }

        [Authorize]
        public ActionResult Test()
        {
            return Content("You're in");
        }
    }
}