using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using Owin;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web
{

	public partial class Startup
	{
	    private void ConfigureIdentityServer(IAppBuilder app, IdentityServerConfiguration configuration)
	    {
	        _logger.Debug("Setting up IdentityServer");

            AntiForgeryConfig.UniqueClaimTypeIdentifier = DasClaimTypes.Id;

            app.Map("/identity", idsrvApp =>
            {
                var factory = new IdentityServerServiceFactory()
                    .UseDasUserService()
                    .UseInMemoryClients(GetClients(configuration))
                    .UseInMemoryScopes(GetScopes())
                    .RegisterDasServices(StructuremapMvc.Container);

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
        private List<Client> GetClients(IdentityServerConfiguration configuration)
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
                    "openid",
                    "profile"
                }
            };

            var employerPortal = new Client
            {
                ClientName = "Das Employer Portal",
                ClientId = "employerportal",
                Flow = Flows.Implicit,
                RequireConsent = false,

                RedirectUris = new List<string>
                {
                    "http://localhost:58887/"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://localhost:58887/"
                },
                AllowedScopes = new List<string>
                {
                    "openid",
                    "profile"
                }
            };

            return new List<Client> { self, employerPortal };
        }
        private List<Scope> GetScopes()
        {
            var scopes = new List<Scope>();

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}