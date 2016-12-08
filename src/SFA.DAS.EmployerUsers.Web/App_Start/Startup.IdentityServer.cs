using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;
using MediatR;
using NLog;
using Owin;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web
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

    public class SecretValidator : ISecretValidator
    {
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            return Task.FromResult(new SecretValidationResult
            {
                Success = true
            });
        }
    }

    public partial class Startup
    {
        private void ConfigureIdentityServer(IAppBuilder app, IdentityServerConfiguration configuration, IRelyingPartyRepository relyingPartyRepository)
        {
            _logger.Debug("Setting up IdentityServer");

            AntiForgeryConfig.UniqueClaimTypeIdentifier = DasClaimTypes.Id;

            app.Map("/identity", idsrvApp =>
            {
                var factory = new IdentityServerServiceFactory()
                    .UseDasUserService()
                    .UseInMemoryClients(GetClients(configuration, relyingPartyRepository))
                    .UseInMemoryScopes(GetScopes())
                    .RegisterDasServices(StructuremapMvc.Container);
                factory.RedirectUriValidator = new Registration<IRedirectUriValidator>((dr) => new StartsWithRedirectUriValidator());

                factory.SecretValidators.Clear();
                factory.SecretValidators.Add(new Registration<ISecretValidator>((dr) => new SecretValidator()));

                //factory.ConfigureDefaultViewService<CustomIdsViewService>(new DefaultViewServiceOptions());

                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Digital Apprenticeship Service",

                    SigningCertificate = LoadCertificate(configuration),

                    Factory = factory,

                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true
                    }
                });
            });
        }


        private X509Certificate2 LoadCertificate(IdentityServerConfiguration configuration)
        {
            var certificatePath = string.Format(@"{0}\bin\DasIDPCert.pfx", AppDomain.CurrentDomain.BaseDirectory);
            _logger.Debug("Loading IDP certificate from {0}", certificatePath);
            return new X509Certificate2(certificatePath, "idsrv3test");

            //TODO: This need fixing to work with new Windows store
            //var storeLocation = (StoreLocation)Enum.Parse(typeof (StoreLocation), configuration.CertificateStore);
            //var store = new X509Store(storeLocation);
            //store.Open(OpenFlags.ReadOnly);
            //try
            //{
            //    var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, configuration.CertificateThumbprint, false);
            //    var certificate = certificates.Count > 0 ? certificates[0] : null;

            //    return certificate;
            //}
            //finally
            //{
            //    store.Close();
            //}
        }
        private List<Client> GetClients(IdentityServerConfiguration configuration, IRelyingPartyRepository relyingPartyRepository)
        {
            var self = new Client
            {
                ClientName = "Das Id Manager",
                ClientId = "idp",
                Flow = Flows.Implicit,
                RequireConsent = false,

                RedirectUris = new List<string>
                {
                    configuration.ApplicationBaseUrl
                },
                PostLogoutRedirectUris = new List<string>
                {
                    configuration.ApplicationBaseUrl
                },
                AllowedScopes = new List<string>
                {
                    "openid", "profile"
                }
            };
            var clients = new List<Client> { self };

            var clientSecret = "super-secret".Sha256();
            var relyingParties = relyingPartyRepository.GetAllAsync().Result;
            clients.AddRange(relyingParties.Select(rp => new Client
            {
                ClientName = rp.Name,
                ClientId = rp.Id,
                Flow = (Flows)rp.Flow, //Flows.AuthorizationCode,
                ClientSecrets = new List<Secret>
                {
                    new Secret(clientSecret)
                },
                RequireConsent = false,
                RedirectUris = new List<string>
                {
                    rp.ApplicationUrl
                },
                PostLogoutRedirectUris = new List<string>
                {
                    rp.LogoutUrl
                },
                AllowedScopes = new List<string>
                {
                    "openid",
                    "profile"
                }
            }));


            return clients;
        }
        private List<Scope> GetScopes()
        {
            var scopes = new List<Scope>();

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}