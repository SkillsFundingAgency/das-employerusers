using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.InMemory;
using Owin;

namespace SFA.DAS.EmployerUsers.Web
{
	public partial class Startup
	{
	    private void ConfigureIdentityServer(IAppBuilder app)
	    {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;

            app.Map("/identity", idsrvApp =>
            {
                var factory = new IdentityServerServiceFactory()
                    .UseInMemoryUsers(GetUsers())
                    .UseInMemoryClients(GetClients())
                    .UseInMemoryScopes(GetScopes());

                //factory.ConfigureDefaultViewService<CustomIdsViewService>(new DefaultViewServiceOptions());

                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Digital Apprentice Service",
                    SigningCertificate = LoadCertificate(),

                    Factory = factory,

                    AuthenticationOptions = new AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true
                    }
                });
            });
        }


        private X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(string.Format(@"{0}\bin\das.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
	    private List<InMemoryUser> GetUsers()
	    {
	        return new List<InMemoryUser>
	        {
	            new InMemoryUser
	            {
	                Username = "test",
	                Password = "password",
	                Subject = "4117c08d-8cf7-4960-b99b-903f627c4d53"
	            }
	        };
	    }
        private List<Client> GetClients()
        {
            var self = new Client
            {
                ClientName = "Das Id Manager",
                ClientId = "mvc",
                Flow = Flows.Implicit,
                RequireConsent = false,

                RedirectUris = new List<string>
                {
                    "https://localhost/"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "https://localhost/"
                },
                AllowedScopes = new List<string>
                {
                    "openid",
                    "profile"
                }
            };

            return new List<Client> { self };
        }
        private List<Scope> GetScopes()
        {
            var scopes = new List<Scope>();

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}