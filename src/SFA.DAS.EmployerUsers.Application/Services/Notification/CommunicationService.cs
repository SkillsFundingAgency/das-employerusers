using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class CommunicationService : ICommunicationService
    {
        private const string ReplyToAddress = "info@sfa.das.gov.uk";

        private readonly INotificationsApi _notificationsApi;
        private readonly ILogger _logger;

        public CommunicationService(INotificationsApi notificationsApi, ILogger logger)
        {
            _notificationsApi = notificationsApi;
            _logger = logger;
        }

        public async Task SendUserRegistrationMessage(User user, string messageId)
        {
            var userAccessCode = GetUserAccessCode(user);
            var email = new Email
            {
                SystemId = Guid.NewGuid().ToString(),
                TemplateId = "UserRegistration",
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                Subject = "Access your apprenticeship levy account",
                Tokens = new Dictionary<string, string>
                {
                    { "AccessCode", userAccessCode.Code },
                    { "CodeExpiry", userAccessCode.ExpiryTime.ToString("d MMMM yyyy") },
                    { "ReturnUrl" , userAccessCode.ReturnUrl }
                }
            };
            try
            {
                await _notificationsApi.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.Error(ex,"SendUserRegistrationMessage: Error while sending email");
            }
            
        }

        public async Task SendUserAccountConfirmationMessage(User user, string messageId)
        {
            await _notificationsApi.SendEmail(new Email
            {
                SystemId = messageId,
                TemplateId = "UserAccountConfirmation",
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                Subject = "Welcome to your apprenticeship levy account"
            });
        }

        public async Task SendAccountLockedMessage(User user, string messageId)
        {
            var userUnlockCode = GetUserUnlockCode(user);
            try
            {
                await _notificationsApi.SendEmail(new Email
                {
                    SystemId = Guid.NewGuid().ToString(),
                    TemplateId = "AccountLocked",
                    RecipientsAddress = user.Email,
                    ReplyToAddress = ReplyToAddress,
                    Subject = "Unlock Code: apprenticeship levy account",
                    Tokens = new Dictionary<string, string>
                {
                    { "UnlockCode", userUnlockCode.Code },
                    { "CodeExpiry", userUnlockCode.ExpiryTime.ToString("d MMMM yyyy") },
                    { "ReturnUrl", userUnlockCode.ReturnUrl }
                }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SendUserRegistrationMessage: Error while sending email");
            }
            
        }

        public async Task ResendActivationCodeMessage(User user, string messageId)
        {
            var userAccessCode = GetUserAccessCode(user);
            try
            {
                await _notificationsApi.SendEmail(new Email
                {
                    SystemId = Guid.NewGuid().ToString(),
                    TemplateId = "ResendActivationCode",
                    RecipientsAddress = user.Email,
                    ReplyToAddress = ReplyToAddress,
                    Subject = "Access your apprenticeship levy account",
                    Tokens = new Dictionary<string, string>
                    {
                        {"AccessCode", userAccessCode.Code},
                        {"CodeExpiry", userAccessCode.ExpiryTime.ToString("d MMMM yyyy")},
                        {"ReturnUrl", userAccessCode.ReturnUrl}
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SendUserRegistrationMessage: Error while sending email");
            }
        }

        public async Task SendUserUnlockedMessage(User user, string messageId)
        {
            await _notificationsApi.SendEmail(new Email
            {
                SystemId = messageId,
                TemplateId = "AccountUnLocked",
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                Subject = "Your account had been unlocked"
            });
        }

        public async Task SendPasswordResetCodeMessage(User user, string messageId)
        {
            var resetCode = GetUserPasswordResetCode(user);
            try
            {
                await _notificationsApi.SendEmail(new Email
                {
                    SystemId = messageId,
                    TemplateId = "PasswordReset",
                    RecipientsAddress = user.Email,
                    ReplyToAddress = ReplyToAddress,
                    Subject = "Reset Password: apprenticeship levy account",
                    Tokens = new Dictionary<string, string>
                {
                    { "Code", resetCode.Code }
                }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "SendUserRegistrationMessage: Error while sending email");
            }
        }

        public async Task SendPasswordResetConfirmationMessage(User user, string messageId)
        {
            await _notificationsApi.SendEmail(new Email
            {
                SystemId = messageId,
                TemplateId = "PasswordResetConfirmation",
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                Subject = "Your password has been reset"
            });
        }

        public async Task SendConfirmEmailChangeMessage(User user, string messageId)
        {
            var code = GetUserConfirmEmailCode(user);
            try
            {
                await _notificationsApi.SendEmail(new Email
                {
                    SystemId = Guid.NewGuid().ToString(),
                    TemplateId = "ConfirmEmailChange",
                    RecipientsAddress = code.PendingValue,
                    ReplyToAddress = ReplyToAddress,
                    Subject = "Change your apprenticeship levy account email address",
                    Tokens = new Dictionary<string, string>
                {
                    { "ConfirmEmailCode", code.Code }
                }
                });
            }
            catch (Exception ex)
        

            {
                _logger.Error(ex, "SendUserRegistrationMessage: Error while sending email");
            }
            
        }
        
		public async Task SendNoAccountToPasswordResetMessage(string emailAddress, string messageId, string registerUrl)
        {
			try
			{
	            await _notificationsApi.SendEmail(new Email
	            {
	                SystemId = messageId,
	                TemplateId = "ForgottenPasswordNoAccount",
	                RecipientsAddress = emailAddress,
	                ReplyToAddress = ReplyToAddress,
	                Subject = "Reset Password: apprenticeship levy account",
	                Tokens = new Dictionary<string, string>
	                {
	                    {"RegisterUrl",registerUrl}
	                }
	            });
			}
			catch(Exception ex)
			{
                _logger.Error(ex, "SendUserRegistrationMessage: Error while sending email");
            }
        }
		
        private SecurityCode GetUserAccessCode(User user)
        {
            return user.SecurityCodes.Where(sc => sc.CodeType == SecurityCodeType.AccessCode)
                                     .OrderByDescending(sc => sc.ExpiryTime)
                                     //.Select(sc => sc.Code)
                                     .FirstOrDefault();
        }
        private SecurityCode GetUserUnlockCode(User user)
        {
            return user.SecurityCodes.Where(sc => sc.CodeType == SecurityCodeType.UnlockCode)
                                     .OrderByDescending(sc => sc.ExpiryTime)
                                     .FirstOrDefault();
        }
        private SecurityCode GetUserPasswordResetCode(User user)
        {
            return user.SecurityCodes.Where(sc => sc.CodeType == SecurityCodeType.PasswordResetCode)
                                     .OrderByDescending(sc => sc.ExpiryTime)
                                     .FirstOrDefault();
        }
        private SecurityCode GetUserConfirmEmailCode(User user)
        {
            return user.SecurityCodes.Where(sc => sc.CodeType == SecurityCodeType.ConfirmEmailCode)
                                     .OrderByDescending(sc => sc.ExpiryTime)
                                     .FirstOrDefault();
        }
    }
}