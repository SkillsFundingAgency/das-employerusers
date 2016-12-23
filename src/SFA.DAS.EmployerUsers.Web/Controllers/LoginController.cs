using System.Web.Mvc;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.ViewModels;
using SFA.DAS.EmployerUsers.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    [RoutePrefix("identity/employer")]
    public class LoginController : Controller
    {
        public ActionResult AuthorizeResponse(AuthorizeResponseViewModel model)
        {
            // Try and stop direct access - probably a better way to do this
            if (model == null || string.IsNullOrEmpty(model.ResponseFormFields))
            {
                return HttpNotFound();
            }
            return View(model);
        }

        public ActionResult Logout(LoggedOutViewModel model, SignOutMessage message)
        {
            return View(new LogOutViewModel
            {
                IdsLogoutModel = model,
                SignOutMessage = message
            });
        }
    }
}