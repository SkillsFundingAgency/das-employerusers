using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using SFA.DAS.EmployerUsers.Api.Orchestrators;
using SFA.DAS.EmployerUsers.Api.Types;

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
        
        [Route("govuk", Name = "GovUk"), HttpGet]
        [Authorize(Roles = "ReadEmployerUsers")]
        public async Task<IHttpActionResult> ById([FromUri]string id)
        {
            return await Show(id);
        }

        [Route("{id}/suspend")]
        [HttpPost]
        [Authorize(Roles = "UpdateEmployerUsers")]
        public async Task<IHttpActionResult> Suspend(string id, [FromBody]ChangedByUserInfo changedByUserInfo)
        {
            SuspendUserResponse response = null;

            try
            {
                response = await _orchestrator.Suspend(id, changedByUserInfo);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            if(string.IsNullOrEmpty(response.Id))
            {
                return NotFound();
            }

            return Ok(response);
        }

        [Route("{id}/resume")]
        [HttpPost]
        [Authorize(Roles = "UpdateEmployerUsers")]
        public async Task<IHttpActionResult> Resume(string id, [FromBody] ChangedByUserInfo changedByUserInfo)
        {
            ResumeUserResponse response = null;

            try
            {
                response = await _orchestrator.Resume(id, changedByUserInfo);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            if (string.IsNullOrEmpty(response.Id))
            {
                return NotFound();
            }

            return Ok(response);
        }

        [Route("{email}", Name = "Update"), HttpPut]
        [Authorize(Roles = "UpdateEmployerUsers")]
        public async Task<IHttpActionResult> Update(string email, [FromBody] UpdateUser updateUser)
        {
            var userResponse = await _orchestrator.UpdateUser(email, updateUser.GovUkIdentifier);

            return Created($"{userResponse.GovUkIdentifier}", userResponse);
        }
    }
}
