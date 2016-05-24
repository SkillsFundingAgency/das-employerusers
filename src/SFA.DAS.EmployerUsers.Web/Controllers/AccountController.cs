using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Web.Authentication;
using IdentityServer3.Core;
using IdentityServer3.Core.Extensions;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    //[RoutePrefix("identity/employer")]
    public class AccountController : Controller
    {
        private readonly AccountOrchestrator _accountOrchestrator;
        private readonly IOwinWrapper _owinWrapper;
        private readonly IConfigurationService _configurationService;

        public AccountController(AccountOrchestrator accountOrchestrator, IOwinWrapper owinWrapper, IConfigurationService configurationService)
        {
            _accountOrchestrator = accountOrchestrator;
            _owinWrapper = owinWrapper;
            _configurationService = configurationService;
        }

        [HttpGet]
        [Route("identity/employer/login")]
        public Task<ActionResult> Login(string id)
        {
            return Task.FromResult<ActionResult>(View(false));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("identity/employer/login")]
        public async Task<ActionResult> Login(string id, LoginViewModel model)
        {

            var success = await _accountOrchestrator.Login(model);
            if (success)
            {
                var signinMessage = _owinWrapper.GetSignInMessage(id);
                return Redirect(signinMessage.ReturnUrl);
            }
            return View(true);
        }

        [HttpGet]
        [Route("identity/employer/register")]
        public async Task<ActionResult> Register()
        {
            return await Task.Run<ActionResult>(() => View(new RegisterViewModel {Valid = true}));
        }

        [HttpPost]
        [Route("identity/employer/register")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var registerResult = await _accountOrchestrator.Register(model);

            if (registerResult)
            {
                return RedirectToAction("Confirm");
            }

            model.ConfirmPassword = string.Empty;
            model.Password = string.Empty;
            model.Valid = false;

            return View("Register", model);
        }

        [HttpPost]
        [Authorize]
        [Route("account/confirm")]
        public async Task<ActionResult> Confirm(AccessCodeViewModel accessCodeViewModel)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var idClaim = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.Id);
            var id = idClaim?.Value;

            var activatedSuccessfully = await _accountOrchestrator.ActivateUser(new AccessCodeViewModel {AccessCode = accessCodeViewModel.AccessCode, UserId = id});

            if (activatedSuccessfully)
            {
                return await RedirectToEmployerPortal();
            }

            return View("Confirm", new AccessCodeViewModel {Valid = false});
        }

        [HttpGet]
        [Authorize]
        [Route("account/confirm")]
        public async Task<ActionResult> Confirm()
        {
            return await Task.Run<ActionResult>(() => View("Confirm", new AccessCodeViewModel {Valid = true}));
        }


        private async Task<ActionResult> RedirectToEmployerPortal()
        {
            var configuration = await _configurationService.Get<EmployerUsersConfiguration>();
            return Redirect(configuration.IdentityServer.EmployerPortalUrl);
        }
    }
}