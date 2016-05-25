using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Services.Notification
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IConfigurationService _configurationService;
        private readonly HttpClient _httpClient;

        public HttpClientWrapper(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _httpClient = new HttpClient { BaseAddress = new Uri(@"http://localhost:53105/") };
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