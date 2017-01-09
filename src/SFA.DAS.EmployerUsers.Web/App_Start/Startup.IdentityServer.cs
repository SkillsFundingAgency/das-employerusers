using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Microsoft.Azure;
using Owin;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Plumbing.Ids;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web
{
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

                factory.ConfigureDefaultViewService<CustomIdsViewService>(new DefaultViewServiceOptions());
                if (!CloudConfigurationManager.GetSetting("EnvironmentName").Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                {
                    factory.AuthorizationCodeStore = new Registration<IAuthorizationCodeStore>(typeof(RedisAuthorizationCodeStore));
                }
                
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Digital Apprenticeship Service",
                   
                    CspOptions = new CspOptions()
                    {
                        FontSrc = "* data:",
                        ImgSrc = "* data:",
                        FrameSrc = "* data:",
                        Enabled = false
                    },
                    SigningCertificate = LoadCertificate(),

                    Factory = factory,

                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true,
                        EnableSignOutPrompt = false,
                        PostSignOutAutoRedirectDelay = 0,
                        SignInMessageThreshold = 1                       
                    }
                });
            });
        }


        private X509Certificate2 LoadCertificate()
        {
            var store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                var thumbprint = CloudConfigurationManager.GetSetting("TokenCertificateThumbprint");
                var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                if (certificates.Count < 1)
                {
                    throw new Exception($"Could not find certificate with thumbprint {thumbprint} in LocalMachine store");
                }

                return certificates[0];
            }
            finally
            {
                store.Close();
            }
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

            var relyingParties = relyingPartyRepository.GetAllAsync().Result;
            clients.AddRange(relyingParties.Select(rp => new Client
            {
                ClientName = rp.Name,
                ClientId = rp.Id,
                Flow = (Flows)rp.Flow,
                ClientSecrets = new List<Secret>
                {
                    new Secret(rp.ClientSecret)
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