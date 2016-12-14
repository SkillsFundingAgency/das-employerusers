using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdentityServer3.Core.ViewModels;

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
    }
}