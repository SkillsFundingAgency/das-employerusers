using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EmployerUsers.Api.Orchestrators;

namespace SFA.DAS.EmployerUsers.Api.Controllers
{
    [RoutePrefix("api/users")]
    public class UserController : ApiController
    {
        private readonly UserOrchestrator _orchestrator;

        public UserController(UserOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route(""), HttpGet]
        public async Task<IHttpActionResult> Index(int pageSize = 1000, int pageNumber = 1)
        {
            var users = await _orchestrator.UsersIndex(pageSize, pageNumber);
            
            return Ok(users);
        }
        [Route("{id}"), HttpGet]
        public async Task<IHttpActionResult> Show(string id)
        {
            var user = await _orchestrator.UserShow(id);
            return Ok(user);
        }
    }
}
