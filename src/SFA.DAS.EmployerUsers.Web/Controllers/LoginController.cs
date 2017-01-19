using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.ViewModels;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    [RoutePrefix("identity/employer")]
    public class LoginController : Controller
    {
        private readonly AccountOrchestrator _accountOrchestrator;
        private readonly IOwinWrapper _owinWrapper;
        private readonly IdentityServerConfiguration _identityServerConfiguration;

        public ActionResult AuthorizeResponse(AuthorizeResponseViewModel model)
        {
            // Try and stop direct access - probably a better way to do this
            if (model == null || string.IsNullOrEmpty(model.ResponseFormFields))
            {
                return HttpNotFound();
            }
            return View(model);
        }

        public LoginController(AccountOrchestrator accountOrchestrator, IOwinWrapper owinWrapper, IdentityServerConfiguration identityServerConfiguration)
        {
            _accountOrchestrator = accountOrchestrator;
            _owinWrapper = owinWrapper;
            _identityServerConfiguration = identityServerConfiguration;
        }

        public ActionResult Logout(LoggedOutViewModel model, SignOutMessage message)
        {
            // I dont know how many times this event will fire
            var url = Task.Run(async () =>
            {
                var returnUrl = await _accountOrchestrator.LogoutUrlForClientId(_owinWrapper.GetIdsClientId());
                return returnUrl;
            }).Result;

            if (!string.IsNullOrEmpty(url))
            {
                message.ReturnUrl = url;
            }
            return View(new LogOutViewModel
            {
                IdsLogoutModel = model,
                SignOutMessage = message
            });
        }
    }
}