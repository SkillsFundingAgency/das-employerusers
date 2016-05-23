using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Web.Authentication;
using IdentityServer3.Core;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    [RoutePrefix("identity/employer")]
    public class AccountController : Controller
    {
        private readonly AccountOrchestrator _accountOrchestrator;
        private readonly IOwinWrapper _owinWrapper;

        public AccountController(AccountOrchestrator accountOrchestrator, IOwinWrapper owinWrapper)
        {
            _accountOrchestrator = accountOrchestrator;
            _owinWrapper = owinWrapper;
        }

        [HttpGet]
        [Route("login")]
        public Task<ActionResult> Login(string id)
        {
            return Task.FromResult<ActionResult>(View(false));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("login")]
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
        [Route("register")]
        public async Task<ActionResult> Register()
        {
            return await Task.Run<ActionResult>(() => View(new RegisterViewModel {Valid = true}));
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            return await Task.Run<ActionResult>(() =>
            {
                var actual = _accountOrchestrator.Register(model);

                if (actual.Result)
                {
                    return RedirectToAction("Confirm");
                }

                model.ConfirmPassword = string.Empty;
                model.Password = string.Empty;
                model.Valid = false;

                return View("Register",model);
            });
        }

        [HttpPost]
        [Authorize]
        [Route("confirm")]
        public async Task<ActionResult> Confirm(AccessCodeViewModel accessCodeViewModel)
        {
            return await Task.Run<ActionResult>(() =>
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var idClaim = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.Id);
                var id = idClaim?.Value;

                var result = _accountOrchestrator.ActivateUser(new AccessCodeViewModel {AccessCode = accessCodeViewModel.AccessCode, UserId = id});

                if (result.Result)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View("Confirm", new AccessCodeViewModel {Valid = false});
            });
        }

        
        [HttpGet]
        [Authorize]
        [Route("confirm")]
        public async Task<ActionResult> Confirm()
        {
            return await Task.Run<ActionResult>(() => View("Confirm", new AccessCodeViewModel {Valid = true}));
        }
    }
}