using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IConfigurationService _configurationService;
        private HttpClient _httpClient;

        public HttpClientWrapper(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task SendMessage(Dictionary<string, string> messageProperties)
        {
            var configuration = await _configurationService.GetAsync<EmployerUsersConfiguration>();
            _httpClient = new HttpClient { BaseAddress = new Uri(configuration.EmployerPortalConfiguration.ApiBaseUrl) };

            var serializeObject = JsonConvert.SerializeObject(messageProperties);
            var resposne = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/notification")
            {
                Content = new StringContent(serializeObject, Encoding.UTF8, "application/json"),
            });
            resposne.EnsureSuccessStatusCode();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}