using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SFA.DAS.EmployerUsers.Api.Controllers
{

    [RoutePrefix("api/status")]
    public class StatusController : ApiController
    {
        [Route("")]
        public IHttpActionResult Index()
        {
            // Do some Infrastructre work here to smoke out any issues.
            return Ok();
        }


    }
}
