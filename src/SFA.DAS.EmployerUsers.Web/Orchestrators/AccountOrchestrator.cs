using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode;
using SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode;
using SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode;
using SFA.DAS.EmployerUsers.Application.Commands.UnlockUser;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress;
using SFA.DAS.EmployerUsers.Application.Queries.IsPasswordResetValid;
using SFA.DAS.EmployerUsers.Application.Queries.IsUserActive;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators
{
    public class AccountOrchestrator : IOrchestrator
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;
        private readonly IOwinWrapper _owinWrapper;


        //TODO : Remove
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
                await _mediator.SendAsync(new RegisterUserCommand
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = registerUserViewModel.FirstName,
                    LastName = registerUserViewModel.LastName,
                    Email = registerUserViewModel.Email,
                    Password = registerUserViewModel.Password,
                    ConfirmPassword = registerUserViewModel.ConfirmPassword,
                    HasAcceptedTermsAndConditions = registerUserViewModel.HasAcceptedTermsAndConditions
                });

                var user = await _mediator.SendAsync(new GetUserByEmailAddressQuery
                {
                    EmailAddress = registerUserViewModel.Email
                });

                _owinWrapper.IssueLoginCookie(user.Id, $"{user.FirstName} {user.LastName}");

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

        public virtual async Task<UnlockUserViewModel> UnlockUser(UnlockUserViewModel unlockUserViewModel)
        {
            try
            {
                await _mediator.SendAsync(new UnlockUserCommand
                {
                    Email = unlockUserViewModel.Email,
                    UnlockCode = unlockUserViewModel.UnlockCode
                });

                await _mediator.SendAsync(new ActivateUserCommand
                {
                    Email = unlockUserViewModel.Email
                });
                
                return unlockUserViewModel;
            }
            catch (InvalidRequestException ex)
            {
                Logger.Info(ex, ex.Message);

                if (ex.ErrorMessages.ContainsKey(nameof(unlockUserViewModel.UnlockCodeExpired)))
                {
                    unlockUserViewModel.UnlockCodeExpired = true;
                }
                unlockUserViewModel.ErrorDictionary = ex.ErrorMessages;
                return unlockUserViewModel;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                unlockUserViewModel.ErrorDictionary = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"", "Unexpected error occured"}
                };
                return unlockUserViewModel;
            }
        }


        public virtual async Task<UnlockUserViewModel> ResendUnlockCode(UnlockUserViewModel model)
        {
            
            try
            {
                await _mediator.SendAsync(new ResendUnlockCodeCommand
                {
                    Email = model.Email
                });

                model.UnlockCodeSent = true;

                return model;
            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
                return model;
            }
            
        }

        public virtual async Task<RequestPasswordResetViewModel> RequestPasswordResetCode(RequestPasswordResetViewModel model)
        {
            try
            {
                await _mediator.SendAsync(new RequestPasswordResetCodeCommand
                {
                    Email = model.Email
                });

                model.ResetCodeSent = true;

                return model;
            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
                return model;
            }
        }

        public virtual async Task<ValidatePasswordResetViewModel> ValidatePasswordResetCode(ValidatePasswordResetViewModel model)
        {
            var isUserActive = await _mediator.SendAsync(new IsPasswordResetCodeValidQuery { Email= model.Email, PasswordResetCode = model.PasswordResetCode});

            return new ValidatePasswordResetViewModel
            {
                Email = model.Email,
                HasExpired = isUserActive.HasExpired,
                IsValid = isUserActive.IsValid
            };
        }
    }
}