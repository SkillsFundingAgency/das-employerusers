using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Web.Authentication;
using IdentityServer3.Core;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    //[RoutePrefix("identity/employer")]
    public class AccountController : ControllerBase
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
        public ActionResult Login(string id)
        {
            return View(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("identity/employer/login")]
        public async Task<ActionResult> Login(string id, LoginViewModel model)
        {
            var result = await _accountOrchestrator.Login(model);

            if (result.Success)
            {
                if (result.RequiresActivation)
                {
                    return RedirectToAction("Confirm");
                }

                var signinMessage = _owinWrapper.GetSignInMessage(id);
                return Redirect(signinMessage.ReturnUrl);
            }

            if (result.AccountIsLocked)
            {
                return RedirectToAction("Unlock");
            }

            return View(true);
        }



        [Route("account/logout")]
        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        [Route("identity/employer/register")]
        [OutputCache(Duration = 0)]
        public ActionResult Register()
        {

            var id = GetLoggedInUserId();

            if (!string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Confirm"); 
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [Route("identity/employer/register")]
        [OutputCache(Duration = 0)]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var id = GetLoggedInUserId();

            if (!string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Confirm");
            }

            var registerResult = await _accountOrchestrator.Register(model);

            if (registerResult.Valid)
            {
                return RedirectToAction("Confirm");
            }

            model.ConfirmPassword = string.Empty;
            model.Password = string.Empty;

            return View("Register", model);
        }
        
        [HttpGet]
        [Authorize]
        [Route("account/confirm")]
        public ActionResult Confirm()
        {
            return View("Confirm", new AccessCodeViewModel { Valid = true });
        }

        [HttpPost]
        [Authorize]
        [Route("account/confirm")]
        public async Task<ActionResult> Confirm(AccessCodeViewModel accessCodeViewModel, string command)
        {
            var id = GetLoggedInUserId();

            if (command.Equals("activate"))
            {
                var activatedSuccessfully = 
                    await
                        _accountOrchestrator.ActivateUser(new AccessCodeViewModel
                        {
                            AccessCode = accessCodeViewModel.AccessCode,
                            UserId = id
                        });

                if (activatedSuccessfully)
                {
                    return await RedirectToEmployerPortal();
                }

                return View("Confirm", new AccessCodeViewModel {Valid = false});
            }
            else
            {
                await _accountOrchestrator.ResendActivationCode(new ResendActivationCodeViewModel { UserId = id });

                return View("Confirm", new AccessCodeViewModel { Valid = true });
            }
        }

        private string GetLoggedInUserId()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var idClaim = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.Id);
            var id = idClaim?.Value;
            return id;
        }
        
        [HttpGet]
        //[Authorize]
        [Route("account/unlock")]
        public ActionResult Unlock()
        {
            return View("Unlock");
        }



        private async Task<ActionResult> RedirectToEmployerPortal()
        {
            var configuration = await _configurationService.GetAsync<EmployerUsersConfiguration>();
            return Redirect(configuration.IdentityServer.EmployerPortalUrl);
        }
    }
}