using System.Web;
using System.Web.Mvc;

namespace TestRP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Login()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            var owinContext = HttpContext.GetOwinContext();
            var authenticationManager = owinContext.Authentication;

            var idToken = authenticationManager.User.FindFirst("id_token").Value;

            authenticationManager.SignOut("Cookies");

            return new RedirectResult($"https://localhost:44334/identity/connect/endsession?id_token_hint={idToken}&post_logout_redirect_uri=http://localhost:17995/");
        }
    }
}