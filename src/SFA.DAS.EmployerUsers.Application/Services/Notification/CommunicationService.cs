using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class CommunicationService : ICommunicationService
    {
        private const string ReplyToAddress = "info@sfa.das.gov.uk";

        private readonly IHttpClientWrapper _httpClientWrapper;

        public CommunicationService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;

        }

        public async Task SendUserRegistrationMessage(User user, string messageId)
        {
            var message = new EmailNotification
            {
                MessageType = "UserRegistration",
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "AccessCode", GetUserAccessCode(user) },
                    { "MessageId", messageId }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendUserAccountConfirmationMessage(User user, string messageId)
        {
            var message = new EmailNotification
            {
                MessageType = "UserAccountConfirmation",
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "MessageId", messageId }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendAccountLockedMessage(User user, string messageId)
        {
            var message = new EmailNotification
            {
                MessageType = "AccountLocked",
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "UnlockCode", GetUserUnlockCode(user) },
                    { "MessageId", messageId }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task ResendActivationCodeMessage(User user, string messageId)
        {
            var message = new EmailNotification
            {
                MessageType = "ResendActivationCode",
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "AccessCode", GetUserAccessCode(user) },
                    { "MessageId", messageId }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendUserUnlockedMessage(User user, string messageId)
        {
            var message = new EmailNotification
            {
                MessageType = "AccountUnLocked",
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "MessageId", messageId }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendPasswordResetCodeMessage(User user, string messageId)
        {
            var resetCode = GetUserPasswordResetCode(user);

            var message = new EmailNotification
            {
                MessageType = "PasswordReset",
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "MessageId", messageId },
                    { "Code", resetCode.Code },
                    { "ExpiryDate", resetCode.ExpiryTime.ToString() }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendPasswordResetConfirmationMessage(User user, string messageId)
        {
            var message = new EmailNotification
            {
                MessageType = "PasswordResetConfirmation",
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = ReplyToAddress,
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "MessageId", messageId }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendConfirmEmailChangeMessage(User user, string messageId)
        {
            var message = new EmailNotification
            {
                MessageType = "ConfirmEmailChange",
                UserId = user.Id,
                RecipientsAddress = user.PendingEmail,
                ReplyToAddress = ReplyToAddress,
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "MessageId", messageId },
                    { "ConfirmEmailCode", GetUserConfirmEmailCode(user) }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }



        private string GetUserAccessCode(User user)
        {
            return user.SecurityCodes.Where(sc => sc.CodeType == SecurityCodeType.AccessCode)
                                     .OrderByDescending(sc => sc.ExpiryTime)
                                     .Select(sc => sc.Code)
                                     .FirstOrDefault();
        }
        private string GetUserUnlockCode(User user)
        {
            return user.SecurityCodes.Where(sc => sc.CodeType == SecurityCodeType.UnlockCode)
                                     .OrderByDescending(sc => sc.ExpiryTime)
                                     .Select(sc => sc.Code)
                                     .FirstOrDefault();
        }
        private SecurityCode GetUserPasswordResetCode(User user)
        {
            return user.SecurityCodes.Where(sc => sc.CodeType == SecurityCodeType.PasswordResetCode)
                                     .OrderByDescending(sc => sc.ExpiryTime)
                                     .FirstOrDefault();
        }
        private string GetUserConfirmEmailCode(User user)
        {
            return user.SecurityCodes.Where(sc => sc.CodeType == SecurityCodeType.ConfirmEmailCode)
                                     .OrderByDescending(sc => sc.ExpiryTime)
                                     .Select(sc => sc.Code)
                                     .FirstOrDefault();
        }
    }
}