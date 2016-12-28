using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using NLog;

namespace SFA.DAS.EmployerUsers.Web.Plumbing.Ids
{
    public class StartsWithRedirectUriValidator : IRedirectUriValidator
    {
        private ILogger _logger = LogManager.GetCurrentClassLogger();

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            _logger.Info($"Attempting to match {requestedUri} for client {client.ClientId}");

            foreach (var uri in client.RedirectUris)
            {
                if (requestedUri.ToLower().StartsWith(uri.ToLower()))
                {
                    _logger.Info($"Matched {requestedUri} to {uri} for client {client.ClientId}");
                    return Task.FromResult(true);
                }
            }

            _logger.Info($"Failed to match {requestedUri} for client {client.ClientId}");
            return Task.FromResult(false);
        }

        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            foreach (var uri in client.PostLogoutRedirectUris)
            {
                if (requestedUri.ToLower().StartsWith(uri.ToLower()))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
    }
}