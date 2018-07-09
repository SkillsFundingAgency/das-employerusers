using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Support.Infrastructure;
using SFA.DAS.Support.Shared.Discovery;
using SFA.DAS.Support.Shared.Navigation;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IEmployerUserRepository _repository;
        private readonly IServiceConfiguration _serviceConfiguration;

        public UserController(IEmployerUserRepository repository, IServiceConfiguration serviceConfiguration, IMenuService menuService,
            IMenuTemplateTransformer menuTemplateTransformer
            ) : base(menuService, menuTemplateTransformer)
        {
            _repository = repository;
            _serviceConfiguration = serviceConfiguration;

        }

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

            MenuPerspective = SupportMenuPerspectives.EmployerUser;
            MenuTransformationIdentifiers = new Dictionary<string, string>() { { "userId", $"{id}" } };
            MenuSelection = "User";


            response.Accounts = await _repository.GetAccounts(id);

            ViewBag.Header = response.Accounts;
            response.AccountsUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerAccount}";


            return View(response);
        }
    }
}