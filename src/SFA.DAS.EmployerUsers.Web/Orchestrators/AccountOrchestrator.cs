using System;
using System.Threading.Tasks;
using IdentityServer3.Core.Extensions;
using MediatR;
using Microsoft.Owin;
using NLog;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators
{
    public class AccountOrchestrator : IOrchestrator
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;
        private readonly IOwinWrapper _owinWrapper;

        public AccountOrchestrator()
        {

        }

        public AccountOrchestrator(IMediator mediator, IOwinWrapper owinWrapper)
        {
            _mediator = mediator;
            _owinWrapper = owinWrapper;
        }

        public virtual async Task<LoginResultModel> Login(LoginViewModel loginViewModel)
        {
            try
            {
                var user = await _mediator.SendAsync(new AuthenticateUserCommand
                {
                    EmailAddress = loginViewModel.EmailAddress,
                    Password = loginViewModel.Password
                });
                if (user == null)
                {
                    return new LoginResultModel { Success = false };
                }

                _owinWrapper.IssueLoginCookie(user.Id, $"{user.FirstName} {user.LastName}");
                _owinWrapper.RemovePartialLoginCookie();

                return new LoginResultModel { Success = true, RequiresActivation = !user.IsActive };
            }
            catch (AccountLockedException ex)
            {
                _logger.Info(ex.Message);
                return new LoginResultModel { AccountIsLocked = true };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new LoginResultModel { Success = false };
            }
        }

        public virtual async Task<bool> Register(RegisterViewModel registerUserViewModel)
        {
            try
            {
                var userId = Guid.NewGuid().ToString();
                await _mediator.SendAsync(new RegisterUserCommand
                {
                    Id = userId,
                    FirstName = registerUserViewModel.FirstName,
                    LastName = registerUserViewModel.LastName,
                    Email = registerUserViewModel.Email,
                    Password = registerUserViewModel.Password,
                    ConfirmPassword = registerUserViewModel.ConfirmPassword
                });


                _owinWrapper.IssueLoginCookie(userId, $"{registerUserViewModel.FirstName} {registerUserViewModel.LastName}");
                //var env = owinContext.Environment;
                //env.IssueLoginCookie(new IdentityServer3.Core.Models.AuthenticatedLogin
                //{
                //    Subject = userId,
                //    Name = $"{registerUserViewModel.FirstName} {registerUserViewModel.LastName}",
                //});
                _owinWrapper.RemovePartialLoginCookie();

                return true;
            }
            catch (InvalidRequestException)
            {
                return false;
            }

        }

        public virtual async Task<bool> ActivateUser(AccessCodeViewModel accessCodeviewModel)
        {
            try
            {
                await _mediator.SendAsync(new ActivateUserCommand
                {
                    AccessCode = accessCodeviewModel.AccessCode,
                    UserId = accessCodeviewModel.UserId
                });

                return true;
            }
            catch (InvalidRequestException)
            {
                return false;
            }

        }
    }
}