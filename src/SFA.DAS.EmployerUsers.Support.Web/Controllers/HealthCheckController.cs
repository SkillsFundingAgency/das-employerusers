using System.Web.Http;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    public class HealthCheckController : ApiController
    {
        [Route("api/HealthCheck")]
        public IHttpActionResult GetStatus()
        {
            return Ok();
        }
    }
}