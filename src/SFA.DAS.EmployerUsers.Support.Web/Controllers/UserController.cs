using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Support.Infrastructure;
using SFA.DAS.Support.Shared.Authentication;
using SFA.DAS.Support.Shared.Challenge;
using SFA.DAS.Support.Shared.Discovery;
using SFA.DAS.Support.Shared.Navigation;
using SFA.DAS.Support.Shared.ViewModels;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    [RoutePrefix("employers")]
    public class UserController : BaseController
    {
        private readonly IEmployerUserRepository _repository;
        private readonly IServiceConfiguration _serviceConfiguration;

        public UserController(IEmployerUserRepository repository,
            IServiceConfiguration serviceConfiguration,
            IMenuService menuService,
            IMenuTemplateTransformer menuTemplateTransformer,
            IChallengeService challengeService,
            IIdentityHandler identityHandler) :
            base(menuService, menuTemplateTransformer, challengeService, identityHandler)
        {
            _repository = repository;
            _serviceConfiguration = serviceConfiguration;
        }

        [Route("users/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return View("_notUnderstood", new { Identifiers = new Dictionary<string, string>() { { "User Id", $"{id}" } } });
            }

            var response = await _repository.Get(id);

            if (response == null)
            {
                return View("_notFound", new { Identifiers = new Dictionary<string, string>() { { "User Id", $"{id}" } } });
            }
            ViewBag.Header = new HeaderViewModel() { Content = new HtmlString($"{response.FirstName} {response.LastName}") }; ;

            MenuPerspective = SupportMenuPerspectives.EmployerUser;
            MenuTransformationIdentifiers = new Dictionary<string, string>() { { "userId", $"{id}" } };
            MenuSelection = "User.Details";

            response.Accounts = await _repository.GetAccounts(id);

            response.AccountsUri = $"/accounts//{{0}}";


            return View(response);
        }
    }
}