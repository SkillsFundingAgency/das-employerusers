using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client
{
    public class EmployerUsersApiClient : IEmployerUsersApiClient
    {
        private readonly IEmployerUsersApiConfiguration _configuration;
        private ISecureHttpClient _secureHttpClient;

        public EmployerUsersApiClient(IEmployerUsersApiConfiguration configuration, ISecureHttpClient secureHttpClient)
        {
            if (secureHttpClient == null)
                throw new ArgumentNullException(nameof(secureHttpClient));

            _configuration = configuration;
            _secureHttpClient = secureHttpClient;
        }

        public EmployerUsersApiClient(EmployerUsersApiConfiguration configuration) : this(configuration, new SecureHttpClient(configuration))
        {
        }

        public async Task<T> GetResource<T>(string resourceUri) where T : IEmployerUsersResource
        {
            var absoluteUri = new Uri(new Uri(_configuration.ApiBaseUrl), resourceUri);
            var json = await _secureHttpClient.GetAsync(absoluteUri.ToString());
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<PagedApiResponseViewModel<UserSummaryViewModel>> GetPageOfEmployerUsers(int pageNumber = 1, int pageSize = 1000)
        {
            return await GetResource<PagedApiResponseViewModel<UserSummaryViewModel>>($"/api/users?pageNumber={pageNumber}&pageSize={pageSize}");
        }

        public async Task<PagedApiResponseViewModel<UserSummaryViewModel>> SearchEmployerUsers(string criteria, int pageNumber = 1, int pageSize = 1000)
        {
            return await GetResource<PagedApiResponseViewModel<UserSummaryViewModel>>($"/api/users/search/{criteria}?pageNumber={pageNumber}&pageSize={pageSize}");
        }
    }
}
