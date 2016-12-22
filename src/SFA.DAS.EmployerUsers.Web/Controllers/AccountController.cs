using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Web.Authentication;
using IdentityServer3.Core;
using Microsoft.Owin.Security;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
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
            var signinMessage = _owinWrapper.GetSignInMessage(id);
            var model = new LoginViewModel
            {
                InvalidLoginAttempt = false,
                ReturnUrl = signinMessage.ReturnUrl
            };
            return View(model);
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

            model.InvalidLoginAttempt = true;
            return View(model);
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
        public async Task<ActionResult> Register(string clientId, string returnUrl)
        {
            

            var loginReturnUrl = Url.Action("Index", "Home", null, Request.Url.Scheme)
                                 + "identity/connect/authorize";
            var isLocalReturnUrl = returnUrl.ToLower().StartsWith(loginReturnUrl.ToLower());
            var model = await _accountOrchestrator.StartRegistration(clientId, returnUrl, isLocalReturnUrl);
            if (!model.Valid)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            var id = GetLoggedInUserId();

            if (!string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Confirm");
            }

            _owinWrapper.ClearSignInMessageCookie();

            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("identity/employer/register")]
        [OutputCache(Duration = 0)]
        public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            var id = GetLoggedInUserId();

            if (!string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Confirm");
            }

            var registerResult = await _accountOrchestrator.Register(model, returnUrl);

            if (registerResult.Valid)
            {
                return RedirectToAction("Confirm");
            }

            model.ConfirmPassword = string.Empty;
            model.Password = string.Empty;
            model.ReturnUrl = returnUrl;

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
            return View("Confirm", new ActivateUserViewModel { Valid = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Route("account/confirm")]
        public async Task<ActionResult> Confirm(ActivateUserViewModel activateUserViewModel, string command)
        {
            var id = GetLoggedInUserId();

            if (command.Equals("activate"))
            {
                activateUserViewModel =
                    await
                        _accountOrchestrator.ActivateUser(new ActivateUserViewModel
                        {
                            AccessCode = activateUserViewModel.AccessCode,
                            UserId = id
                        });

                if (activateUserViewModel.Valid)
                {
                    return Redirect(activateUserViewModel.ReturnUrl);
                }

                return View("Confirm", new ActivateUserViewModel { Valid = false });
            }
            else
            {
                var result = await _accountOrchestrator.ResendActivationCode(new ResendActivationCodeViewModel { UserId = id });

                return View("Confirm", new ActivateUserViewModel { Valid = result });
            }
        }



        [HttpGet]
        [AttemptAuthorise]
        [Route("account/unlock")]
        public ActionResult Unlock()
        {
            var email = GetLoggedInUserEmail();
            var model = new UnlockUserViewModel { Email = email };
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

            if (string.IsNullOrEmpty(requestPasswordResetViewModel.Email) || !requestPasswordResetViewModel.Valid)
            {
                return View("ForgottenCredentials", requestPasswordResetViewModel);
            }

            return View("ResetPassword", new PasswordResetViewModel { Email = requestPasswordResetViewModel.Email });
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("identity/employer/resetpassword")]
        public async Task<ActionResult> ResetPassword(PasswordResetViewModel model)
        {
            model = await _accountOrchestrator.ResetPassword(model);

            if (model.Valid)
            {
                return await RedirectToEmployerPortal();
            }

            return View("ResetPassword", model);
        }



        [HttpGet]
        [Authorize]
        [Route("account/changeemail")]
        public async Task<ActionResult> ChangeEmail(string clientId, string returnUrl)
        {
            var model = await _accountOrchestrator.StartRequestChangeEmail(clientId, returnUrl);
            if (!model.Valid)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("account/changeemail")]
        public async Task<ActionResult> ChangeEmail(ChangeEmailViewModel model, string clientId, string returnUrl)
        {
            model.UserId = GetLoggedInUserId();
            model.ClientId = clientId;
            model.ReturnUrl = returnUrl;

            await _accountOrchestrator.RequestChangeEmail(model);

            return RedirectToAction("ConfirmChangeEmail");
        }

        [HttpGet]
        [Authorize]
        [Route("account/confirmchangeemail")]
        public ActionResult ConfirmChangeEmail()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("account/confirmchangeemail")]
        public async Task<ActionResult> ConfirmChangeEmail(ConfirmChangeEmailViewModel model)
        {
            model.UserId = GetLoggedInUserId();

            model = await _accountOrchestrator.ConfirmChangeEmail(model);
            if (model.Valid)
            {
                return Redirect(model.ReturnUrl);
            }

            model.SecurityCode = string.Empty;
            model.Password = string.Empty;
            return View(model);
        }


        [HttpGet]
        [Authorize]
        [Route("account/changepassword")]
        public async Task<ActionResult> ChangePassword(string clientId, string returnUrl)
        {
            var model = await _accountOrchestrator.StartChangePassword(clientId, returnUrl);
            if (!model.Valid)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [Route("account/changepassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model, string clientId, string returnUrl)
        {
            model.UserId = GetLoggedInUserId();

            model = await _accountOrchestrator.ChangePassword(model);
            if (model.Valid)
            {
                return Redirect(returnUrl);
            }

            model.CurrentPassword = string.Empty;
            model.NewPassword = string.Empty;
            model.ConfirmPassword = string.Empty;
            return View(model);
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