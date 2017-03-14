using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Application.Commands.ChangeEmail;
using SFA.DAS.EmployerUsers.Application.Commands.ChangePassword;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Application.Commands.RequestChangeEmail;
using SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode;
using SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode;
using SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode;
using SFA.DAS.EmployerUsers.Application.Commands.UnlockUser;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByHashedId;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Application.Queries.IsUserActive;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Models.SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators
{
    public class AccountOrchestrator
    {


        private readonly IMediator _mediator;
        private readonly IOwinWrapper _owinWrapper;
        private readonly ILogger _logger;


        //Needed for testing
        protected AccountOrchestrator()
        {

        }

        public AccountOrchestrator(IMediator mediator, IOwinWrapper owinWrapper, ILogger logger)
        {
            _mediator = mediator;
            _owinWrapper = owinWrapper;
            _logger = logger;
        }

        public virtual async Task<OrchestratorResponse<LoginResultModel>> Login(LoginViewModel loginViewModel)
        {
            try
            {
                var user = await _mediator.SendAsync(new AuthenticateUserCommand
                {
                    EmailAddress = loginViewModel.EmailAddress,
                    Password = loginViewModel.Password,
                    ReturnUrl = loginViewModel.ReturnUrl
                });
                if (user == null)
                {
                    _logger.Warn(
                        $"Failed login attempt for email address '{loginViewModel.EmailAddress}' originating from {loginViewModel.OriginatingAddress}");
                    return new OrchestratorResponse<LoginResultModel>
                    {
                        Status = HttpStatusCode.BadRequest,
                        FlashMessage = new FlashMessageViewModel
                        {
                            ErrorMessages = new Dictionary<string, string> {
                                {
                                    "", "Invalid credentials"
                                } },
                            Severity = FlashMessageSeverityLevel.Error,
                            Headline = "Errors to fix",
                            Message = "Check the following details:"
                        },
                        Data = new LoginResultModel { Success = false }
                    };
                }

                LoginUser(user.Id, user.FirstName, user.LastName);

                return new OrchestratorResponse<LoginResultModel>
                {
                    Data = new LoginResultModel { Success = true, RequiresActivation = !user.IsActive }
                };
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse<LoginResultModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    FlashMessage = new FlashMessageViewModel
                    {
                        ErrorMessages = ex.ErrorMessages,
                        Severity = FlashMessageSeverityLevel.Error,
                        Headline = "Errors to fix",
                        Message = "Check the following details:"
                    },
                    Data = new LoginResultModel { Success = false }
                };
            }
            catch (AccountLockedException ex)
            {
                _logger.Info(ex.Message);

                return new OrchestratorResponse<LoginResultModel> { Data = new LoginResultModel { AccountIsLocked = true } };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new OrchestratorResponse<LoginResultModel> { Data = new LoginResultModel { Success = false } };
            }
        }

        public virtual async Task<RegisterViewModel> StartRegistration(string clientId, string returnUrl, bool isLocalReturnUrl)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                ClientId = clientId
            };

            if (!isLocalReturnUrl)
            {
                await ValidateClientIdReturnUrlCombo(clientId, returnUrl, model);
            }

            return model;
        }

        public virtual async Task<OrchestratorResponse<RegisterViewModel>> Register(RegisterViewModel registerUserViewModel, string returnUrl)
        {
            var returnModel = new OrchestratorResponse<RegisterViewModel>
            {
                Data = registerUserViewModel
            };
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
                    HasAcceptedTermsAndConditions = registerUserViewModel.HasAcceptedTermsAndConditions,
                    ReturnUrl = returnUrl
                });

                var user = await _mediator.SendAsync(new GetUserByEmailAddressQuery
                {
                    EmailAddress = registerUserViewModel.Email
                });

                LoginUser(user.Id, user.FirstName, user.LastName);
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex, ex.Message);
                returnModel.Data.ErrorDictionary = ex.ErrorMessages;

                returnModel.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    Severity = FlashMessageSeverityLevel.Error,
                    ErrorMessages = ex.ErrorMessages
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                returnModel.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    Severity = FlashMessageSeverityLevel.Error,
                    ErrorMessages = new Dictionary<string, string> { { "", "Unexpected error occured" } }
                };
            }

            returnModel.Data = registerUserViewModel;

            return returnModel;
        }

        public virtual async Task<ActivateUserViewModel> ActivateUser(ActivateUserViewModel model)
        {
            try
            {
                var result = await _mediator.SendAsync(new ActivateUserCommand
                {
                    AccessCode = model.AccessCode,
                    UserId = model.UserId
                });


                model.ReturnUrl = result.ReturnUrl;
                return model;
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex, ex.Message);


                model.ErrorDictionary = ex.ErrorMessages;
                return model;
            }
        }
        public virtual async Task<bool> ResendLastConfirmationCode(ConfirmChangeEmailViewModel model)
        {
            try
            {
                await _mediator.SendAsync(new ResendActivationCodeCommand
                {
                    UserId = model.UserId
                });

                return true;
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
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
                _logger.Info(ex, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return false;
            }
        }

        public virtual async Task<bool> RequestConfirmAccount(string userId)
        {
            try
            {
                var isUserActive = await _mediator.SendAsync(new IsUserActiveQuery { UserId = userId });
                return !isUserActive;
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

            }
            return false;
        }

        public virtual async Task<OrchestratorResponse<UnlockUserViewModel>> UnlockUser(UnlockUserViewModel unlockUserViewModel)
        {
            try
            {

                var unlockResponse = await _mediator.SendAsync(new UnlockUserCommand
                {
                    Email = unlockUserViewModel.Email,
                    UnlockCode = unlockUserViewModel.UnlockCode
                });

                await _mediator.SendAsync(new ActivateUserCommand
                {
                    Email = unlockUserViewModel.Email
                });
                if (unlockResponse != null)
                {
                    unlockUserViewModel.ReturnUrl = unlockResponse.UnlockCode.ReturnUrl;
                }
                return new OrchestratorResponse<UnlockUserViewModel> { Data = unlockUserViewModel };
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex, ex.Message);

                if (ex.ErrorMessages.ContainsKey(nameof(unlockUserViewModel.UnlockCodeExpired)))
                {
                    unlockUserViewModel.UnlockCodeExpired = true;
                }
                unlockUserViewModel.ErrorDictionary = ex.ErrorMessages;


                var flashMessage = new FlashMessageViewModel
                {
                    ErrorMessages = ex.ErrorMessages,
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    Severity = FlashMessageSeverityLevel.Error
                };


                return new OrchestratorResponse<UnlockUserViewModel> { Data = unlockUserViewModel, FlashMessage = flashMessage };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                unlockUserViewModel.ErrorDictionary = new Dictionary<string, string>
                {
                    {"", "Unexpected error occured"}
                };
                var flashMessage = new FlashMessageViewModel
                {
                    ErrorMessages = unlockUserViewModel.ErrorDictionary,
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    Severity = FlashMessageSeverityLevel.Error
                };
                return new OrchestratorResponse<UnlockUserViewModel> { Data = unlockUserViewModel, FlashMessage = flashMessage };
            }
        }

        public virtual async Task<OrchestratorResponse<UnlockUserViewModel>> ResendUnlockCode(UnlockUserViewModel model)
        {

            try
            {
                await _mediator.SendAsync(new ResendUnlockCodeCommand
                {
                    Email = model.Email,
                    ReturnUrl = model.ReturnUrl
                });

                model.UnlockCodeSent = true;

                var flashMessage = new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "Unlock your account",
                    SubMessage = "We've resent an email with a code to unlock your account"
                };

                return new OrchestratorResponse<UnlockUserViewModel> { Data = model, FlashMessage = flashMessage };
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex, ex.Message);
                model.ErrorDictionary = ex.ErrorMessages;

                var flashMessage = new FlashMessageViewModel
                {
                    ErrorMessages = ex.ErrorMessages,
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    Severity = FlashMessageSeverityLevel.Error
                };

                return new OrchestratorResponse<UnlockUserViewModel> { Data = model, FlashMessage = flashMessage }; ;
            }

        }

        public virtual async Task<RequestPasswordResetViewModel> RequestPasswordResetCode(RequestPasswordResetViewModel model)
        {
            try
            {
                await _mediator.SendAsync(new RequestPasswordResetCodeCommand
                {
                    Email = model.Email,
                    ReturnUrl = model.ReturnUrl
                });

                model.ResetCodeSent = true;
            }
            catch (InvalidRequestException ex)
            {
                _logger.Info(ex, ex.Message);
                model.ErrorDictionary = ex.ErrorMessages;
            }
            return model;
        }

        public virtual async Task<OrchestratorResponse<PasswordResetViewModel>> ResetPassword(PasswordResetViewModel model)
        {
            var response = new OrchestratorResponse<PasswordResetViewModel>();
            try
            {
                var resetResponse = await _mediator.SendAsync(new PasswordResetCommand
                {
                    Email = model.Email,
                    PasswordResetCode = model.PasswordResetCode ?? null,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword
                });

                var user = await _mediator.SendAsync(new GetUserByEmailAddressQuery
                {
                    EmailAddress = model.Email
                });

                LoginUser(user.Id, user.FirstName, user.LastName);
                if (resetResponse?.ResetCode != null)
                {
                    model.ReturnUrl = resetResponse.ResetCode.ReturnUrl;
                }

                response.Data = model;

                return response;
            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                response.Data = model;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }

            return response;
        }

        public virtual async Task<OrchestratorResponse<ChangeEmailViewModel>> StartRequestChangeEmail(string clientId, string returnUrl)
        {
            var response = new OrchestratorResponse<ChangeEmailViewModel>();
            var model = new ChangeEmailViewModel
            {
                ReturnUrl = returnUrl + "?userCancelled=true",
                ClientId = clientId
            };
            try
            {


                await ValidateClientIdReturnUrlCombo(clientId, returnUrl, model);

                response.Data = model;
                if (!response.Data.Valid)
                {
                    response.Status = HttpStatusCode.BadRequest;
                }
                return response;
            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
                response.Status = HttpStatusCode.BadRequest;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                response.Status = HttpStatusCode.InternalServerError;

            }
            response.Data = model;

            return response;
        }
        public virtual async Task<OrchestratorResponse<ChangeEmailViewModel>> RequestChangeEmail(ChangeEmailViewModel model)
        {
            var response = new OrchestratorResponse<ChangeEmailViewModel>() { Data = new ChangeEmailViewModel() };

            try
            {
                var isClientValid = await ValidateClientIdReturnUrlCombo(model.ClientId, model.ReturnUrl, model);
                if (!isClientValid)
                {
                    response.Data = model;
                    return response;
                }

                await _mediator.SendAsync(new RequestChangeEmailCommand
                {
                    UserId = model.UserId,
                    NewEmailAddress = model.NewEmailAddress,
                    ConfirmEmailAddress = model.ConfirmEmailAddress,
                    ReturnUrl = model.ReturnUrl
                });

            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
                response.Status = HttpStatusCode.BadRequest;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                model.ErrorDictionary.Add("", ex.Message);
                response.Status = HttpStatusCode.InternalServerError;

            }
            response.Data = model;

            return response;
        }

        public virtual async Task<ConfirmChangeEmailViewModel> ConfirmChangeEmail(ConfirmChangeEmailViewModel model)
        {
            try
            {
                var user = await _mediator.SendAsync(new GetUserByIdQuery
                {
                    UserId = model.UserId
                });

                var changeEmailResult = await _mediator.SendAsync(new ChangeEmailCommand
                {
                    User = user,
                    SecurityCode = model.SecurityCode,
                    Password = model.Password
                });
                model.ReturnUrl = changeEmailResult.ReturnUrl;
            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
            }
            catch (Exception ex)
            {
                model.ErrorDictionary.Add("", ex.Message);
            }
            return model;
        }

        public virtual async Task<OrchestratorResponse<ChangePasswordViewModel>> StartChangePassword(string clientId, string returnUrl)
        {
            var model = new ChangePasswordViewModel
            {
                ReturnUrl = returnUrl + "?userCancelled=true",
                ClientId = clientId
            };

            await ValidateClientIdReturnUrlCombo(clientId, returnUrl, model);

            var response = new OrchestratorResponse<ChangePasswordViewModel>() { Data = model };
            if (!response.Data.Valid)
            {
                response.Status = HttpStatusCode.BadRequest;
            }

            return response;
        }
        public virtual async Task<ChangePasswordViewModel> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var isClientValid = await ValidateClientIdReturnUrlCombo(model.ClientId, model.ReturnUrl, model);
                if (!isClientValid)
                {
                    return model;
                }

                var user = await _mediator.SendAsync(new GetUserByIdQuery
                {
                    UserId = model.UserId
                });

                await _mediator.SendAsync(new ChangePasswordCommand
                {
                    User = user,
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword
                });

            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
            }
            catch (Exception ex)
            {
                model.ErrorDictionary.Add("", ex.Message);
            }

            return model;
        }


        public async Task<RequestPasswordResetViewModel> StartForgottenPassword(string clientId, string returnUrl)
        {
            var model = new RequestPasswordResetViewModel();
            
            var relyingParty = await _mediator.SendAsync(new GetRelyingPartyQuery { Id = clientId });

            if (relyingParty == null)
            {
                AddInvalidClientIdReturnUrlMessage(model);
            }
            else
            {
                model.ReturnUrl = !string.IsNullOrEmpty(returnUrl) ? returnUrl : relyingParty.ApplicationUrl;
            }

            return model;
        }

        public async Task<string> LogoutUrlForClientId(string clientId)
        {

            var relyingParty = await _mediator.SendAsync(new GetRelyingPartyQuery { Id = clientId });

            if (relyingParty == null)
            {
                return "";
            }
            else
            {
                return relyingParty.ApplicationUrl;
            }
        }

        private void LoginUser(string id, string firstName, string lastName)
        {
            _owinWrapper.IssueLoginCookie(id, $"{firstName} {lastName}");

            _owinWrapper.RemovePartialLoginCookie();
        }

        private async Task<bool> ValidateClientIdReturnUrlCombo(string clientId, string returnUrl, ViewModelBase model)
        {
            var isValid = await IsValidClientIdReturnUrlCombo(clientId, returnUrl);
            if (!isValid)
            {
                AddInvalidClientIdReturnUrlMessage(model);
                return false;
            }
            return true;
        }

        private async Task<bool> IsValidClientIdReturnUrlCombo(string clientId, string returnUrl)
        {
            var relyingParty = await _mediator.SendAsync(new GetRelyingPartyQuery { Id = clientId });
            return relyingParty != null && returnUrl.StartsWith(relyingParty.ApplicationUrl);
        }

        private static void AddInvalidClientIdReturnUrlMessage(ViewModelBase model)
        {
            if (model.ErrorDictionary == null)
            {
                model.ErrorDictionary = new Dictionary<string, string>();
            }
            model.ErrorDictionary.Add("", "Invalid client id / return url");
        }


        public void RedirectToRelyingParty(string getIdsClientId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> StartRedirectToAuhorizedClientEndpoint(string clientId, string returnUrl)
        {
            var relyingParty = await _mediator.SendAsync(new GetRelyingPartyQuery { Id = clientId });

            return relyingParty != null ? relyingParty.LoginCallbackUrl : string.Empty;
        }

        public async Task<OrchestratorResponse<PasswordResetViewModel>>  ForgottenPasswordFromEmail(string hashedUserId)
        {
            var response = new OrchestratorResponse<PasswordResetViewModel>();

            try
            {
                var user = await _mediator.SendAsync(new GetUserByHashedIdQuery {HashedUserId = hashedUserId});

                if (user == null)
                {
                    response.Status = HttpStatusCode.BadRequest;
                    return response;
                }

                response.Data = new PasswordResetViewModel {Email = user.Email};
            }
            catch (InvalidRequestException ex)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = "Errors to fix",
                    Message = "Check the following details:",
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }

            return response;
        }
    }
}