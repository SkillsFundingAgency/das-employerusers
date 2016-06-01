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
            var messageProperties = new Dictionary<string, string>
            {
                {"AccessCode", user.AccessCode},
                {"UserId", user.Id},
                {"MessageId", messageId},
                {"messagetype", "SendEmail"},
                {"toEmail", user.Email},
                {"fromEmail", "info@sfa.das.gov.uk"}
            };

            await _httpClientWrapper.SendMessage(messageProperties);
        }

        public async Task SendUserAccountConfirmationMessage(User user, string messageId)
        {
            var messageProperties = new Dictionary<string, string>
            {
                {"body", "Account Created Sucessfully"},
                {"UserId", user.Id},
                {"MessageId", messageId},
                {"messagetype", "SendEmail"},
                {"toEmail", user.Email},
                {"fromEmail", "info@sfa.das.gov.uk"}
            };

            await _httpClientWrapper.SendMessage(messageProperties);
        }

        public async Task SendAccountLockedMessage(User user, string messageId)
        {
            var messageProperties = new Dictionary<string, string>
            {
                {"body", user.UnlockCode},
                {"UserId", user.Id},
                {"MessageId", messageId},
                {"messagetype", "SendEmail"},
                {"toEmail", user.Email},
                {"fromEmail", "info@sfa.das.gov.uk"}
            };

            await _httpClientWrapper.SendMessage(messageProperties);
        }
    }
}