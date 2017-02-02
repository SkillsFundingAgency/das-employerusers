using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SFA.DAS.EmployerUsers.Api.Orchestrators;

namespace SFA.DAS.EmployerUsers.Api.Controllers
{
    public class SearchController : ApiController
    {
        private readonly SearchOrchestrator _orchestrator;

        public SearchController(SearchOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        public IHttpActionResult Search(string query, int pageSize = 1000, int pageNumber = 1)
        {
            // Search for users where the query is contained in Email and FullName
            return Ok();
        }
    }
}
