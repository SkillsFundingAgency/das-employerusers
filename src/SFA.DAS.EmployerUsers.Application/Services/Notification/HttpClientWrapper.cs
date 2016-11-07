using System;
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

        public HttpClientWrapper(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task SendMessage<T>(T content)
        {
            using (var httpClient = await CreateHttpClient())
            {
                var serializeObject = JsonConvert.SerializeObject(content);
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/api/email")
                {
                    Content = new StringContent(serializeObject, Encoding.UTF8, "application/json")
                });
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task<HttpClient> CreateHttpClient()
        {
            var configuration = await _configurationService.GetAsync<EmployerUsersConfiguration>();
            return new HttpClient
            {
                BaseAddress = new Uri(configuration.EmployerPortalConfiguration.ApiBaseUrl)
            };
        }
    }
}