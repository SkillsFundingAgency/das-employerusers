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
        [HttpGet, Route("")]
        public IHttpActionResult Index()
        {
            return Ok();
        }

        [Route("{random}"), HttpGet]
        public IHttpActionResult Show(string random)
        {
            return Ok(random);
        }
    }
}
