using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.Support.Shared;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    [Authorize(Roles = "das-support-portal")]
    [RoutePrefix("api/status")]
    public class StatusController : ApiController
    {
        [AllowAnonymous]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                ServiceName = AddServiceName(),
                ServiceVersion = AddServiceVersion(),
                ServiceTime = AddServerTime(),
                Request = AddRequestContext()
            });
        }

        private static string AddServiceVersion()
        {
            try
            {
                return Assembly.GetExecutingAssembly().Version();
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }

        private static string AddRequestContext()
        {
            try
            {
                return $" {HttpContext.Current.Request.HttpMethod}: {HttpContext.Current.Request.RawUrl}";
            }
            catch
            {
                return "Unknown";
            }
        }

        private static DateTimeOffset AddServerTime()
        {
            return DateTimeOffset.UtcNow;
        }

        private static string AddServiceName()
        {
            try
            {
                return "SFA DAS Employer Users Support Site";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}