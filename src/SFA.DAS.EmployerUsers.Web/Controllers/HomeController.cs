using System.Web.Mvc;
using NLog;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly IdentityServerConfiguration _identityServerConfiguration;
        private readonly ILogger _logger;

        public HomeController(IOwinWrapper owinWrapper, IdentityServerConfiguration identityServerConfiguration, ILogger logger)
        {
            _owinWrapper = owinWrapper;
            _identityServerConfiguration = identityServerConfiguration;
            _logger = logger;
        }

        public ActionResult Index()
        {
            var returnUrl = _owinWrapper.GetIdsReturnUrl();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                Logger.Info($"HomeController:Index - Redirecting user out of iDams to {returnUrl}");
                return Redirect(returnUrl);
            }
            returnUrl = _identityServerConfiguration.EmployerPortalUrl;
            Logger.Info($"HomeController:Index - Redirecting user out of iDams (brute force) to {returnUrl}");
            return View(_identityServerConfiguration.EmployerPortalUrl);
        }

        public ActionResult CatchAll(string path)
        {
            return RedirectToAction("NotFound", "Error", new {path});
        }
        
    }
}