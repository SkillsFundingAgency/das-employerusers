using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
            
        }

        [HttpGet]
        public async Task<ActionResult> Register()
        {
            return await Task.Run<ActionResult>(() => View());
        }

        [HttpPost]
        public async Task<ActionResult> Register(dynamic model)
        {
            return await Task.Run<ActionResult>(() => View());
        }
    }
}