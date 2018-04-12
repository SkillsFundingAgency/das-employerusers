using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EmployerUsers.Support.Application.Handlers;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        private readonly IEmployerUserHandler _handler;

        public SearchController(IEmployerUserHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        [Route("users/{pagesize}/{pagenumber}")]
        public async Task<IHttpActionResult> Users(int pageSize, int pageNumber)
        {
            var model = await _handler.FindSearchItems(pageSize,pageNumber);
            return Json(model);
        }

        [HttpGet]
        [Route("users/totalcount/{pageSize}")]
        public async Task<IHttpActionResult> UsersTotalCount(int pageSize)
        {
            var users = await _handler.TotalUserRecords(pageSize);
            return Json(users);
        }

    }
}