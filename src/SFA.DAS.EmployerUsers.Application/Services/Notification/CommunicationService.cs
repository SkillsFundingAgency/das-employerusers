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
            var message = BuildCoreEmailNotification(user, messageId, "UserRegistration");

            message.Data.Add("AccessCode", user.AccessCode);

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendUserAccountConfirmationMessage(User user, string messageId)
        {
            var message = BuildCoreEmailNotification(user, messageId, "UserAccountConfirmation");

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendAccountLockedMessage(User user, string messageId)
        {
            var message = BuildCoreEmailNotification(user, messageId, "AccountLocked");

            message.Data.Add("UnlockCode", user.UnlockCode);

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task ResendActivationCodeMessage(User user, string messageId)
        {
            var message = BuildCoreEmailNotification(user, messageId, "ResendActivationCode");

            message.Data.Add("AccessCode", user.AccessCode);

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendUserUnlockedMessage(User user, string messageId)
        {
            var message = BuildCoreEmailNotification(user, messageId, "AccountUnLocked");

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendPasswordResetCodeMessage(User user, string messageId)
        {
            var message = BuildCoreEmailNotification(user, messageId, "PasswordReset");

            message.Data.Add("Code", user.PasswordResetCode);
            message.Data.Add("ExpiryDate", user.PasswordResetCodeExpiry.Value.ToString());

            await _httpClientWrapper.SendMessage(message);
        }

        public async Task SendPasswordResetConfirmationMessage(User user, string messageId)
        {
            var message = BuildCoreEmailNotification(user, messageId, "PasswordResetConfirmation");

            await _httpClientWrapper.SendMessage(message);
        }

        private EmailNotification BuildCoreEmailNotification(User user, string messageId, string messageType)
        {
            return new EmailNotification
            {
                MessageType = messageType,
                UserId = user.Id,
                RecipientsAddress = user.Email,
                ReplyToAddress = "info@sfa.das.gov.uk",
                ForceFormat = true,
                Data = new Dictionary<string, string>
                {
                    { "MessageId", messageId }
                }
            };

        }
    }
}