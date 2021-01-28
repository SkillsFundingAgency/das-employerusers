using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client
{
    public class EmployerUsersApiClient : IEmployerUsersApiClient
    {
        private readonly IEmployerUsersApiConfiguration _configuration;
        private readonly ISecureHttpClient _secureHttpClient;

        public EmployerUsersApiClient(IEmployerUsersApiConfiguration configuration) : this(configuration, new SecureHttpClient(configuration))
        {
        }

        public EmployerUsersApiClient(IEmployerUsersApiConfiguration configuration, ISecureHttpClient secureHttpClient)
        {
            if (secureHttpClient == null)
                throw new ArgumentNullException(nameof(secureHttpClient));

            _configuration = configuration;
            _secureHttpClient = secureHttpClient;
        }

        public async Task<T> GetResource<T>(string resourceUri) where T : IEmployerUsersResource
        {
            var absoluteUri = Combine(_configuration.ApiBaseUrl, resourceUri);
            var json = await _secureHttpClient.GetAsync(absoluteUri);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<PagedApiResponseViewModel<UserSummaryViewModel>> GetPageOfEmployerUsers(int pageNumber = 1, int pageSize = 1000)
        {
            return await GetResource<PagedApiResponseViewModel<UserSummaryViewModel>>($"/api/users?pageNumber={pageNumber}&pageSize={pageSize}");
        }

        public async Task<PagedApiResponseViewModel<UserSummaryViewModel>> SearchEmployerUsers(string criteria, int pageNumber = 1, int pageSize = 1000)
        {
            return await GetResource<PagedApiResponseViewModel<UserSummaryViewModel>>($"/api/users/search/{SanitiseUrl(criteria)}/?pageNumber={pageNumber}&pageSize={pageSize}");
        }

        private static string SanitiseUrl(string criteria)
        {
            return Uri.EscapeDataString(criteria).Replace(".", "%2E").Replace(" ", "%20");
        }

        public static string Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return $"{uri1}/{uri2}";
        }

        public async Task<SuspendUserResponse> SuspendUser(string id)
        {
            var absoluteUri = Combine(_configuration.ApiBaseUrl, $"/api/users/{id}/suspend");
            var response = await _secureHttpClient.PostAsync(absoluteUri, new StringContent(JsonConvert.SerializeObject(new { Id = id })));

            return JsonConvert.DeserializeObject<SuspendUserResponse>(response);
        }

        public async Task<ResumeUserResponse> ResumeUser(string id)
        {
            var absoluteUri = Combine(_configuration.ApiBaseUrl, $"/api/users/{id}/resume");
            var response = await _secureHttpClient.PostAsync(absoluteUri, new StringContent(JsonConvert.SerializeObject(new { Id = id })));

            return JsonConvert.DeserializeObject<ResumeUserResponse>(response);
        }
    }
}
