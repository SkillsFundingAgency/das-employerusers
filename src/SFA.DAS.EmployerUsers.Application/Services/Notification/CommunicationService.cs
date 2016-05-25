using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class CommunicationService : ICommunicationService
    {
        
        private readonly HttpClient _httpClient;

        public CommunicationService(IConfigurationService configurationService)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(@"http://localhost:53105/") };
        }

        public Task SendUserRegistrationMessage(User user)
        {
            /*
             new Dictionary<string, string>
            {
                { "AccessCode" , registerUser.AccessCode },
                { "UserId" , registerUser.Id },
                { "MessageId" , Guid.NewGuid().ToString() },
                { "messagetype" , "SendEmail"},
                { "toEmail", message.Email},
                { "fromEmail", "info@sfa.das.gov.uk"}
            }
             */
            throw new NotImplementedException();
        }

        public async Task SendMessage(Dictionary<string, string> messageProperties)
        {
            var serializeObject = JsonConvert.SerializeObject(messageProperties);
            var resposne = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/notification")
            {
                Content = new StringContent(serializeObject, Encoding.UTF8, "application/json"),
            });
            resposne.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}