using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFA.DAS.EmployerUsers.Api.Client
{
    internal class SecureHttpClient : ISecureHttpClient
    {
        private readonly IEmployerUsersApiConfiguration _configuration;

        internal SecureHttpClient(IEmployerUsersApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual async Task<string> GetAsync(string url)
        {
            var accessToken = await GetAccessToken();

            using (var store = new ClientCertificateStore(new X509Store(StoreName.My, StoreLocation.CurrentUser)))
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                if (!string.IsNullOrWhiteSpace(_configuration.ClientCertificateThumbprint))
                {
                    var certificate = store.FindCertificateByThumbprint(_configuration.ClientCertificateThumbprint);
                    if (certificate != null)
                    {
                        handler.ClientCertificates.Add(certificate);
                    }
                }

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public virtual async Task<string> PostAsync(string url, HttpContent content)
        {
            var accessToken = await GetAccessToken();

            using (var store = new ClientCertificateStore(new X509Store(StoreName.My, StoreLocation.CurrentUser)))
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                if (!string.IsNullOrWhiteSpace(_configuration.ClientCertificateThumbprint))
                {
                    var certificate = store.FindCertificateByThumbprint(_configuration.ClientCertificateThumbprint);
                    if (certificate != null)
                    {
                        handler.ClientCertificates.Add(certificate);
                    }
                }

                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task<string> GetAccessToken()
        {
            var accessToken = IsClientCredentialConfiguration(_configuration.ClientId, _configuration.ClientSecret, _configuration.Tenant)
                ? await GetClientCredentialAuthenticationResult(_configuration.ClientId, _configuration.ClientSecret, _configuration.IdentifierUri, _configuration.Tenant)
                : await GetManagedIdentityAuthenticationResult(_configuration.IdentifierUri);

            return accessToken;
        }

        private async Task<string> GetClientCredentialAuthenticationResult(string clientId, string clientSecret, string resource, string tenant)
        {
            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, clientSecret);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(resource, clientCredential);
            return result.AccessToken;
        }

        private async Task<string> GetManagedIdentityAuthenticationResult(string resource)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            return await azureServiceTokenProvider.GetAccessTokenAsync(resource);
        }

        private bool IsClientCredentialConfiguration(string clientId, string clientSecret, string tenant)
        {
            return !string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret) && !string.IsNullOrEmpty(tenant);
        }
    }
}