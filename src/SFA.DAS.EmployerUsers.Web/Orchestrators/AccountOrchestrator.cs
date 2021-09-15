using MediatR;
using NLog;
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
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParties;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty;
using SFA.DAS.EmployerUsers.Application.Queries.GetUnlockCodeLength;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByHashedId;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Application.Queries.IsUserActive;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Models.SFA.DAS.EAS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Web.Orchestrators
{
    public class AccountOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IOwinWrapper _owinWrapper;
        private readonly ILogger _logger;

        private const string ProblemHeadline = "There is a problem";

        protected AccountOrchestrator()
        {
            // Needed for testing
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
                    _logger.Info(
                        $"Failed login attempt for email address '{loginViewModel.EmailAddress}' originating from {loginViewModel.OriginatingAddress}");
                    return new OrchestratorResponse<LoginResultModel>
                    {
                        Status = HttpStatusCode.BadRequest,
                        FlashMessage = new FlashMessageViewModel
                        {
                            Headline = ProblemHeadline,
                            ErrorMessages = new Dictionary<string, string>
                            {
                                {
                                    "", "Invalid credentials"
                                }
                            },
                            Severity = FlashMessageSeverityLevel.Error
                        },
                        Data = new LoginResultModel {Success = false}
                    };
                }

                LoginUser(user.Id, user.FirstName, user.LastName);

                return new OrchestratorResponse<LoginResultModel>
                {
                    Data = new LoginResultModel {Success = true, RequiresActivation = !user.IsActive}
                };
            }
            catch (InvalidRequestException ex)
            {
                return new OrchestratorResponse<LoginResultModel>
                {
                    Status = HttpStatusCode.BadRequest,
                    FlashMessage = new FlashMessageViewModel
                    {
                        Headline = ProblemHeadline,
                        ErrorMessages = ex.ErrorMessages,
                        Severity = FlashMessageSeverityLevel.Error
                    },
                    Data = new LoginResultModel {Success = false}
                };
            }
            catch (AccountLockedException ex)
            {
                _logger.Info(ex.Message);

                return new OrchestratorResponse<LoginResultModel>
                {
                    Data = new LoginResultModel { AccountIsLocked = true }
                };
            }
            catch (AccountSuspendedException ex)
            {
                _logger.Info(ex.Message);

                return new OrchestratorResponse<LoginResultModel>
                {
                    Status = HttpStatusCode.Forbidden,
                    FlashMessage = new FlashMessageViewModel
                    {
                        Headline = ProblemHeadline,
                        ErrorMessages = new Dictionary<string,string> {{string.Empty, "There was a problem logging into your account" }},
                        Severity = FlashMessageSeverityLevel.Error
                    },
                    Data = new LoginResultModel { AccountIsSuspended = true }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new OrchestratorResponse<LoginResultModel>
                {
                    Data = new LoginResultModel { Success = false }
                };
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

        public virtual async Task<OrchestratorResponse<RegisterViewModel>> Register(RegisterViewModel model, string returnUrl)
        {
            var response = new OrchestratorResponse<RegisterViewModel>{ Data = model };

            try
            {
                await _mediator.SendAsync(new RegisterUserCommand
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    HasAcceptedTermsAndConditions = model.HasAcceptedTermsAndConditions,
                    ReturnUrl = returnUrl
                });

                var user = await _mediator.SendAsync(new GetUserByEmailAddressQuery
                {
                    EmailAddress = model.Email
                });

                LoginUser(user.Id, user.FirstName, user.LastName);
            }
            catch (InvalidRequestException ex)
            {
                response.Data.ErrorDictionary = ex.ErrorMessages;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    Severity = FlashMessageSeverityLevel.Error,
                    ErrorMessages = ex.ErrorMessages
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                model.ErrorDictionary.Add("", ex.Message);
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = new Dictionary<string, string> { { "", "Unexpected error occured" } },
                    Severity = FlashMessageSeverityLevel.Error,
                };
                response.Exception = ex;
            }

            return response;
        }

        public virtual async Task<OrchestratorResponse<ActivateUserViewModel>> ActivateUser(ActivateUserViewModel model)
        {
            var response = new OrchestratorResponse<ActivateUserViewModel> { Data = model };

            try
            {
                var result = await _mediator.SendAsync(new ActivateUserCommand
                {
                    AccessCode = model.AccessCode,
                    UserId = model.UserId
                });

                response.Data.ReturnUrl = result.ReturnUrl;
            }
            catch (InvalidRequestException ex)
            {
                response.Data.ErrorDictionary = ex.ErrorMessages;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    Severity = FlashMessageSeverityLevel.Error,
                    ErrorMessages = ex.ErrorMessages
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
                model.ErrorDictionary.Add("", ex.Message);
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = new Dictionary<string, string> { { "", "Unexpected error occured" } },
                    Severity = FlashMessageSeverityLevel.Error,
                };
                response.Exception = ex;
            }

            return response;
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

        public virtual async Task<OrchestratorResponse<UnlockUserViewModel>> UnlockUser(UnlockUserViewModel model)
        {
            var response = new OrchestratorResponse<UnlockUserViewModel> { Data = model };

            try
            {
                var unlockResponse = await _mediator.SendAsync(new UnlockUserCommand
                {
                    Email = model.Email,
                    UnlockCode = model.UnlockCode,
                    ReturnUrl = model.ReturnUrl
                });

                await _mediator.SendAsync(new ActivateUserCommand
                {
                    Email = model.Email
                });

                if (unlockResponse != null)
                {
                    model.ReturnUrl = unlockResponse.UnlockCode.ReturnUrl;
                }
            }
            catch (InvalidRequestException ex)
            {
                if (ex.ErrorMessages.ContainsKey(nameof(model.UnlockCodeExpired)))
                {
                    model.UnlockCodeExpired = true;
                }
                
                model.ErrorDictionary = ex.ErrorMessages;
                response.FlashMessage = new FlashMessageViewModel
                {
                    ErrorMessages = ex.ErrorMessages,
                    Headline = ProblemHeadline,
                    Severity = FlashMessageSeverityLevel.Error
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                model.ErrorDictionary.Add("", ex.Message);
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = new Dictionary<string, string> { { "", "Unexpected error occured" } },
                    Severity = FlashMessageSeverityLevel.Error,
                };
                response.Exception = ex;
            }

            return response;
        }

        public virtual async Task<OrchestratorResponse<UnlockUserViewModel>> ResendUnlockCode(UnlockUserViewModel model)
        {
            var response = new OrchestratorResponse<UnlockUserViewModel> { Data = model };

            try
            {
                await _mediator.SendAsync(new ResendUnlockCodeCommand
                {
                    Email = model.Email,
                    ReturnUrl = model.ReturnUrl
                });

                model.UnlockCodeSent = true;

                response.FlashMessage = new FlashMessageViewModel
                {
                    Severity = FlashMessageSeverityLevel.Success,
                    Headline = "Unlock your account",
                    SubMessage = "We've resent an email with a code to unlock your account"
                };
            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
            }

            return response;
        }

        public virtual async Task<OrchestratorResponse<RequestPasswordResetViewModel>> RequestPasswordResetCode(RequestPasswordResetViewModel model)
        {
            var response = new OrchestratorResponse<RequestPasswordResetViewModel> { Data = model };

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
                model.ErrorDictionary = ex.ErrorMessages;
                response.FlashMessage = new FlashMessageViewModel
                {
                    ErrorMessages = ex.ErrorMessages,
                    Headline = ProblemHeadline,
                    Severity = FlashMessageSeverityLevel.Error
                };
            }

            return response;
        }

        public virtual async Task<OrchestratorResponse<EnterResetCodeViewModel>> ValidateResetCode(EnterResetCodeViewModel model)
        {
            var response = new OrchestratorResponse<EnterResetCodeViewModel> { Data = model };

            try
            {
                var resetResponse = await _mediator.SendAsync(new ValidatePasswordResetCodeCommand
                {
                    Email = model.Email,
                    PasswordResetCode = model.PasswordResetCode ?? null
                });

                var user = await _mediator.SendAsync(new GetUserByEmailAddressQuery
                {
                    EmailAddress = model.Email
                });

                if (resetResponse?.ResetCode != null)
                {
                    model.ReturnUrl = resetResponse.ResetCode.ReturnUrl;
                }

                response.Data.UnlockCodeLength = await GetUnlockCodeLength();
            }
            catch(ExceededLimitPasswordResetCodeException ex)
            {
                _logger.Error(ex, ex.Message);
                response.Exception = ex;
            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }

            if (!response.Data.Valid)
            {
                response.Data.PasswordResetCode = string.Empty;
            }

            return response;
        }

        public virtual async Task<OrchestratorResponse<PasswordResetViewModel>> ResetPassword(PasswordResetViewModel model)
        {
            var response = new OrchestratorResponse<PasswordResetViewModel> { Data = model };

            try
            {
                var resetResponse = await _mediator.SendAsync(new PasswordResetCommand
                {
                    Email = model.Email,
                    PasswordResetCode = model.PasswordResetCode ?? null,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword
                });

                if (resetResponse?.ResetCode != null)
                {
                    model.ReturnUrl = resetResponse.ResetCode.ReturnUrl;
                }

                response.Data.UnlockCodeLength = await GetUnlockCodeLength();
            }
            catch(InvalidPasswordResetCodeException ex)
            {
                _logger.Error(ex, ex.Message);
                response.Exception = ex;
            }
            catch (InvalidRequestException ex)
            {
                model.ErrorDictionary = ex.ErrorMessages;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }

            if (!response.Data.Valid)
            {
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
            }

            return response;
        }

        public virtual async Task<OrchestratorResponse<ChangeEmailViewModel>> StartRequestChangeEmail(string clientId, string returnUrl)
        {
            var response = new OrchestratorResponse<ChangeEmailViewModel>
            {
                Data = new ChangeEmailViewModel
                {
                    ReturnUrl = returnUrl + "?userCancelled=true",
                    ClientId = clientId
                }
            };

            try
            {
                await ValidateClientIdReturnUrlCombo(clientId, returnUrl, response.Data);
                if (!response.Data.Valid)
                {
                    response.Status = HttpStatusCode.BadRequest;
                }
            }
            catch (InvalidRequestException ex)
            {
                response.Data.ErrorDictionary = ex.ErrorMessages;
                response.Status = HttpStatusCode.BadRequest;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                response.Data.ErrorDictionary.Add("", ex.Message);
                response.Status = HttpStatusCode.InternalServerError;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = new Dictionary<string, string> { { "", "Unexpected error occured" } },
                    Severity = FlashMessageSeverityLevel.Error,
                };
                response.Exception = ex;

            }
           
            return response;
        }
        public virtual async Task<OrchestratorResponse<ChangeEmailViewModel>> RequestChangeEmail(ChangeEmailViewModel model)
        {
            var response = new OrchestratorResponse<ChangeEmailViewModel>() { Data = model };

            try
            {
                var isClientValid = await ValidateClientIdReturnUrlCombo(model.ClientId, model.ReturnUrl, model);
                if (!isClientValid)
                {
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
                    Headline = ProblemHeadline,
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                model.ErrorDictionary.Add("", ex.Message);
                response.Status = HttpStatusCode.InternalServerError;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = new Dictionary<string, string> { { "", "Unexpected error occured" } },
                    Severity = FlashMessageSeverityLevel.Error,
                };
                response.Exception = ex;
            }

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

        public virtual async Task<OrchestratorResponse<ChangePasswordViewModel>> ChangePassword(ChangePasswordViewModel model)
        {
            var response = new OrchestratorResponse<ChangePasswordViewModel>() { Data = model };

            try
            {
                var isClientValid = await ValidateClientIdReturnUrlCombo(model.ClientId, model.ReturnUrl, model);
                if (!isClientValid)
                {
                    return response;
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
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                model.ErrorDictionary.Add("", ex.Message);
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = new Dictionary<string, string> { { "", "Unexpected error occured" } },
                    Severity = FlashMessageSeverityLevel.Error,
                };
                response.Exception = ex;
            }

            if(!response.Data.Valid)
            {
                response.Data.CurrentPassword = string.Empty;
                response.Data.NewPassword = string.Empty;
                response.Data.ConfirmPassword = string.Empty;
            }

            return response;
        }

        public async Task<RequestPasswordResetViewModel> StartForgottenPassword(string id, string clientId, string returnUrl)
        {
            var model = new RequestPasswordResetViewModel()
            {
                SignInId = id,
                ClientId = clientId
            };
            
            var relyingParty = await _mediator.SendAsync(new GetRelyingPartyQuery { Id = model.ClientId });

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

        public async Task<IEnumerable<string>> GetRelyingPartyLogoutUrls()
        {
            var relyingParties = await _mediator.SendAsync(new GetRelyingPartiesQuery());
            return relyingParties.Select(x => x.LogoutUrl);
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

        public async Task<string> StartRedirectToAuhorizedClientEndpoint(string clientId, string returnUrl)
        {
            var relyingParty = await _mediator.SendAsync(new GetRelyingPartyQuery { Id = clientId });

            return relyingParty != null ? relyingParty.LoginCallbackUrl : string.Empty;
        }

        public async Task<OrchestratorResponse<EnterResetCodeViewModel>> ForgottenPasswordFromEmail(string hashedUserId)
        {
            var response = new OrchestratorResponse<EnterResetCodeViewModel>();

            try
            {
                var user = await _mediator.SendAsync(new GetUserByHashedIdQuery {HashedUserId = hashedUserId});

                if (user == null)
                {
                    response.Status = HttpStatusCode.BadRequest;
                    return response;
                }

                response.Data = new EnterResetCodeViewModel { Email = user.Email, UnlockCodeLength = await GetUnlockCodeLength() };
            }
            catch (InvalidRequestException ex)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.FlashMessage = new FlashMessageViewModel
                {
                    Headline = ProblemHeadline,
                    ErrorMessages = ex.ErrorMessages,
                    Severity = FlashMessageSeverityLevel.Error
                };
                response.Exception = ex;
            }

            return response;
        }

        public virtual async Task<int> GetUnlockCodeLength()
        {
            var model = await _mediator.SendAsync(new GetUnlockCodeQuery());

            return model.UnlockCodeLength;
        }
    }
}