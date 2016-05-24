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
        [Route("Login")]
        public ActionResult Login()
        {
            return RedirectToAction("Index");
        }
    }
}