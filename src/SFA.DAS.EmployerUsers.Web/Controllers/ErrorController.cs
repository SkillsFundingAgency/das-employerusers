using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public ActionResult General()
        {
            return View();
        }
        public ActionResult NotFound(string path = null)
        {
            return View();
        }
    }
}
