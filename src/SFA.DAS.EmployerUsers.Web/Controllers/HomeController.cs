using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IOwinWrapper _owinWrapper;

        public HomeController(IOwinWrapper owinWrapper)
        {
            _owinWrapper = owinWrapper;
        }

        public ActionResult Index()
        {
            var returnUrl = _owinWrapper.GetIdsReturnUrl();

            return Redirect(returnUrl);
        }

        public ActionResult CatchAll(string path)
        {
            return RedirectToAction("NotFound", "Error", new {path});
        }
        
    }
}