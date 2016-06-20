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
using SFA.DAS.EmployerUsers.WebClientComponents;
using System;

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
            model.OriginatingAddress = Request.UserHostAddress;
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
        [Route("account/register")]
        [OutputCache(Duration = 0)]
        [AttemptAuthorise]
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
        [ValidateAntiForgeryToken]
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
        public async Task<ActionResult> Confirm()
        {
            var userId = GetLoggedInUserId();
            var confirmationRequired = await _accountOrchestrator.RequestConfirmAccount(userId);
            if (!confirmationRequired)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("Confirm", new AccessCodeViewModel {Valid = true});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

                return View("Confirm", new AccessCodeViewModel { Valid = false });
            }
            else
            {
                var result = await _accountOrchestrator.ResendActivationCode(new ResendActivationCodeViewModel { UserId = id });

                return View("Confirm", new AccessCodeViewModel { Valid = result });
            }
        }

        [HttpGet]
        [AttemptAuthorise]
        [Route("account/unlock")]
        public ActionResult Unlock()
        {
            var email = GetLoggedInUserEmail();
            var model = new UnlockUserViewModel {Email = email};
            return View("Unlock", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("account/unlock")]
        public async Task<ActionResult> Unlock(UnlockUserViewModel unlockUserViewModel, string command)
        {

            if (command.ToLower() == "resend")
            {
                var result = await _accountOrchestrator.ResendUnlockCode(unlockUserViewModel);
                
                return View("Unlock", result);
            }
            else
            {
                var result = await _accountOrchestrator.UnlockUser(unlockUserViewModel);

                if (result.Valid)
                {
                    return await RedirectToEmployerPortal();
                }
                unlockUserViewModel.UnlockCode = string.Empty;
                return View("Unlock", unlockUserViewModel);
            }
        }

        [HttpGet]
        [Route("account/forgottencredentials")]
        public ActionResult ForgottenCredentials()
        {
            return View("ForgottenCredentials", new RequestPasswordResetViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("account/forgottencredentials")]
        public async Task<ActionResult> ForgottenCredentials(RequestPasswordResetViewModel requestPasswordResetViewModel)
        {
            requestPasswordResetViewModel = await _accountOrchestrator.RequestPasswordResetCode(requestPasswordResetViewModel);

            if (string.IsNullOrEmpty(requestPasswordResetViewModel.Email))
            {
                return View("ForgottenCredentials", requestPasswordResetViewModel);
            }

            return View("ResetPassword", new PasswordResetViewModel {Email = requestPasswordResetViewModel.Email});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("account/resetpassword")]
        public async Task<ActionResult> ResetPassword(PasswordResetViewModel model)
        {
            model = await _accountOrchestrator.ResetPassword(model);

            if (model.Valid)
            {
                return await RedirectToEmployerPortal();
            }

            return View("ResetPassword", model);
        }

        private string GetLoggedInUserId()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var idClaim = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == DasClaimTypes.Id);
            if (idClaim == null)
            {
                idClaim = claimsIdentity?.Claims.FirstOrDefault(c => c.Type == Constants.ClaimTypes.Subject);
            }
            var id = idClaim?.Value;
            return id;
        }

        private string GetLoggedInUserEmail()
        {
            var claimsIdentity = User?.Identity as ClaimsIdentity;
            var idClaim = claimsIdentity?.Claims?.FirstOrDefault(c => c.Type == DasClaimTypes.Email);
            
            var id = idClaim?.Value;
            return id;
        }


        private async Task<ActionResult> RedirectToEmployerPortal()
        {
            var configuration = await _configurationService.GetAsync<EmployerUsersConfiguration>();
            return Redirect(configuration.IdentityServer.EmployerPortalUrl);
        }
    }
}