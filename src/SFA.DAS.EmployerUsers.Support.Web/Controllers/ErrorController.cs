using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Error()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View();
        }


        public ActionResult NotFound()
        {
            Response.StatusCode = (int) HttpStatusCode.NotFound;
            return View("Error");
        }

        public ActionResult BadRequest()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return View("Error");
        }


    }
}