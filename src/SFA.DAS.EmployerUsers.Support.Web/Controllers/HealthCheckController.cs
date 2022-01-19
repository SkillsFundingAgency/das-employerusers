using System.Web.Http;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    public class HealthCheckController : ApiController
    {
        [AllowAnonymous]
        [Route("api/HealthCheck")]
        public IHttpActionResult GetStatus()
        {
            return Ok();
        }
    }
}