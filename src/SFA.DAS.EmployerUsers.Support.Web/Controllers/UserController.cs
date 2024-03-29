﻿using System.IdentityModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Support.Infrastructure;
using SFA.DAS.Support.Shared.Discovery;

namespace SFA.DAS.EmployerUsers.Support.Web.Controllers
{
    [Authorize(Roles = "das-support-portal")]
    public class UserController : Controller
    {
        private readonly IEmployerUserRepository _repository;

        public UserController(IEmployerUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActionResult> Header(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new BadRequestException();

            var response = await _repository.Get(id);

            if (response == null) return HttpNotFound();
            return View("SubHeader", response);
        }

        public async Task<ActionResult> Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new BadRequestException();

            var response = await _repository.Get(id);

            if (response == null) return HttpNotFound();

            response.Accounts = await _repository.GetAccounts(id);
            response.AccountsUri = $"/resource/index/{{0}}?key={SupportServiceResourceKey.EmployerAccount}";

            return View(response);
        }
    }
}