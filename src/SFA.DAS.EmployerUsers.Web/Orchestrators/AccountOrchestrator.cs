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
using SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode;
using SFA.DAS.EmployerUsers.Application.Queries.IsUserActive;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators
{
    public class AccountOrchestrator : IOrchestrator
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

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
                    Logger.Warn($"Failed login attempt for email address '{loginViewModel.EmailAddress}' originating from {loginViewModel.OriginatingAddress}");
                    return new LoginResultModel { Success = false };
                }

                _owinWrapper.IssueLoginCookie(user.Id, $"{user.FirstName} {user.LastName}");
                _owinWrapper.RemovePartialLoginCookie();

                return new LoginResultModel { Success = true, RequiresActivation = !user.IsActive };
            }
            catch (AccountLockedException ex)
            {
                Logger.Info(ex.Message);
                return new LoginResultModel { AccountIsLocked = true };
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return new LoginResultModel { Success = false };
            }
        }

        public virtual async Task<RegisterViewModel> Register(RegisterViewModel registerUserViewModel)
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

                _owinWrapper.IssueLoginCookie(userId,
                    $"{registerUserViewModel.FirstName} {registerUserViewModel.LastName}");

                _owinWrapper.RemovePartialLoginCookie();
            }
            catch (InvalidRequestException ex)
            {
                Logger.Info(ex, ex.Message);
                registerUserViewModel.ErrorDictionary = ex.ErrorMessages;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                registerUserViewModel.ErrorDictionary = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"", "Unexpected error occured"}
                };
            }

            return registerUserViewModel;
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
            catch (InvalidRequestException ex)
            {
                Logger.Info(ex, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        public virtual async Task<bool> ResendActivationCode(ResendActivationCodeViewModel resendActivationCodeViewModel)
        {
            try
            {
                await _mediator.SendAsync(new ResendActivationCodeCommand
                {
                    UserId = resendActivationCodeViewModel.UserId
                });

                return true;
            }
            catch (InvalidRequestException ex)
            {
                Logger.Info(ex, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        public virtual async Task<bool> RequestConfirmAccount(string userId)
        {
            var isUserActive = await _mediator.SendAsync(new IsUserActiveQuery {UserId = userId});
            return !isUserActive;
        }
    }
}