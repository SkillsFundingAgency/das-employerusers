using System;
using System.Collections.Generic;
using System.Linq;
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

        [Route("{id}/suspend")]
        [HttpPost]
        //[Authorize(Roles = "UpdateEmployerUsers")]
        public async Task<IHttpActionResult> Suspend(string id)
        {
            SuspendUserResponse response = null;

            try
            {
                response = await _orchestrator.Suspend(id);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

            if (response.HasError)
            {
                var modelState = new ModelStateDictionary();
                response.Errors.ToList().ForEach(error => modelState.AddModelError(error.Key, error.Value));
                return BadRequest(modelState);
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
        public async Task<IHttpActionResult> Resume(string id)
        {
            ResumeUserResponse response = null;

            try
            {
                response = await _orchestrator.Resume(id);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

            if (response.HasError)
            {
                var modelState = new ModelStateDictionary();
                response.Errors.ToList().ForEach(error => modelState.AddModelError(error.Key, error.Value));
                return BadRequest(modelState);
            }

            if (string.IsNullOrEmpty(response.Id))
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
