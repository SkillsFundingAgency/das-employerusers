using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class CommunicationService : ICommunicationService
    {
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
                ReplyToAddress = "info@sfa.das.gov.uk",
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "AccessCode", user.AccessCode },
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
                ReplyToAddress = "info@sfa.das.gov.uk",
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
                ReplyToAddress = "info@sfa.das.gov.uk",
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "UnlockCode", user.UnlockCode },
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
                ReplyToAddress = "info@sfa.das.gov.uk",
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "AccessCode", user.AccessCode },
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
                ReplyToAddress = "info@sfa.das.gov.uk",
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
            var message = new EmailNotification
            {
                MessageType = "PasswordReset",
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = "info@sfa.das.gov.uk",
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "MessageId", messageId },
                    { "Code", user.PasswordResetCode },
                    { "ExpiryDate", user.PasswordResetCodeExpiry.Value.ToString() }
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
                ReplyToAddress = "info@sfa.das.gov.uk",
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "MessageId", messageId }
                }
            };

            await _httpClientWrapper.SendMessage(message);
        }
    }
}