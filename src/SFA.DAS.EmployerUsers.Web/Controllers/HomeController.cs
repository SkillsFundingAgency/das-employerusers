using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Test()
        {
            return Content("You're in");
        }
    }
}