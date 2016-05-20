using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountOrchestrator _accountOrchestrator;
        
        public AccountController(AccountOrchestrator accountOrchestrator)
        {
            _accountOrchestrator = accountOrchestrator;
        }

        [HttpGet]
        [Route("identity/employer/login")]
        public Task<ActionResult> Login()
        {
            return Task.FromResult<ActionResult>(View());
        }

        [HttpGet]
        public async Task<ActionResult> Register()
        {
            return await Task.Run<ActionResult>(() => View(new RegisterViewModel {Valid = true}));
        }

        [HttpPost]
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
        public async Task<ActionResult> Confirm(AccessCodeViewModel accessCodeViewModel)
        {
            return await Task.Run<ActionResult>(() =>
            {
                var result = _accountOrchestrator.ActivateUser(new AccessCodeViewModel {AccessCode = accessCodeViewModel.AccessCode, UserId = accessCodeViewModel.UserId});

                if (result.Result)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View("Confirm", new AccessCodeViewModel {Valid = false});
            });
        }

        [HttpGet]
        public async Task<ActionResult> Confirm()
        {
            return await Task.Run<ActionResult>(() => View("Confirm", new AccessCodeViewModel {Valid = true}));
        }
    }
}