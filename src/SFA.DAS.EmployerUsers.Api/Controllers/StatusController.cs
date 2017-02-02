using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EmployerUsers.Api.Orchestrators;

namespace SFA.DAS.EmployerUsers.Api.Controllers
{

    [RoutePrefix("api/status")]
    public class StatusController : ApiController
    {
        private readonly UserOrchestrator _orchestrator;

        public StatusController(UserOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("")]
        public async Task <IHttpActionResult> Index()
        {
            try
            {
                // Do some Infrastructre work here to smoke out any issues.
                var users = await _orchestrator.UsersIndex(1, 1);
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }


    }
}
