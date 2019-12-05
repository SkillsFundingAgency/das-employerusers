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
        [Authorize(Roles = "ReadEmployerUsers")]
        public async Task<IHttpActionResult> Index(int pageSize = 1000, int pageNumber = 1)
        {
            var users = await _orchestrator.UsersIndex(pageSize, pageNumber);
            users.Data.Data.ForEach(x => x.Href = Url.Route("Show", new { x.Id }));
            return Ok(users.Data);
        }

        [Route("email", Name = "Email"), HttpGet]
        [Authorize(Roles = "ReadEmployerUsers")]
        public async Task<IHttpActionResult> Email(string emailAddress)
        {
            var user = await _orchestrator.UserByEmail(emailAddress);

            if (user.Data == null)
            {
                return NotFound();
            }

            return Ok(user.Data);
        }

        [Route("{id}", Name = "Show"), HttpGet]
        [Authorize(Roles = "ReadEmployerUsers")]
        public async Task<IHttpActionResult> Show(string id)
        {
            var user = await _orchestrator.UserShow(id);

            if (user.Data == null)
            {
                return NotFound();
            }

            return Ok(user.Data);
        }
    }
}
