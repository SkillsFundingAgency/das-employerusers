using System.Web.Mvc;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        [AuthoriseActiveUser]
        [Route("Login")]
        public ActionResult Login()
        {
            return RedirectToAction("Index");
        }
    }
}