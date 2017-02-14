using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EmployerUsers.Api.Orchestrators;

namespace SFA.DAS.EmployerUsers.Api.Controllers
{
    [RoutePrefix("api/users/search")]
    public class SearchController : ApiController
    {
        private readonly SearchOrchestrator _orchestrator;

        public SearchController(SearchOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("{criteria}"), HttpGet]
        [Authorize(Roles = "ReadEmployerUsers")]
        public async Task<IHttpActionResult> Search(string criteria, int pageSize = 1000, int pageNumber = 1)
        {
            var users = await _orchestrator.UserSearch(criteria, pageSize, pageNumber);
            users.Data.Data.ForEach(x => x.Href = Url.Route("Show", new { x.Id }));
            return Ok(users.Data);
        }
    }
}
