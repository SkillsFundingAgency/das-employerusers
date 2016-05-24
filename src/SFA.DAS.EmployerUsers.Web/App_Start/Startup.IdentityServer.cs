using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Models;
using Owin;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;

namespace SFA.DAS.EmployerUsers.Web
{
    internal class DisposableAction : IDisposable
    {
        private readonly Action _onDispose;

        public DisposableAction(Action onDispose = null)
        {
            this._onDispose = onDispose;
        }

        public void Dispose()
        {
            if (this._onDispose == null)
                return;
            this._onDispose();
        }
    }
    internal class TestLogger : ILogProvider
    {
        private static readonly IDisposable NoopDisposableInstance = (IDisposable)new DisposableAction((Action)null);
        private readonly Lazy<OpenNdc> _lazyOpenNdcMethod;
        private readonly Lazy<OpenMdc> _lazyOpenMdcMethod;

        protected delegate IDisposable OpenNdc(string message);

        protected delegate IDisposable OpenMdc(string key, string value);

        public TestLogger()
        {
            this._lazyOpenNdcMethod = new Lazy<OpenNdc>(new Func<OpenNdc>(this.GetOpenNdcMethod));
            this._lazyOpenMdcMethod = new Lazy<OpenMdc>(new Func<OpenMdc>(this.GetOpenMdcMethod));
        }
        public Logger GetLogger(string name)
        {
            return (Logger) Log;
        }

        private bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            if (messageFunc == null)
            {
                return true;
            }

            var format = messageFunc();
            var message = string.Format(format, formatParameters);
            if (exception != null)
            {
                message += $"\n    {exception}";
            }

            Debug.Print($"IDS [{logLevel}]: {message}");
            return true;
        }

        public IDisposable OpenNestedContext(string message)
        {
            return this._lazyOpenNdcMethod.Value(message);
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            return this._lazyOpenMdcMethod.Value(key, value);
        }

        protected virtual OpenNdc GetOpenNdcMethod()
        {
            return (OpenNdc)(_ => NoopDisposableInstance);
        }

        protected virtual OpenMdc GetOpenMdcMethod()
        {
            return (OpenMdc)((_, __) => NoopDisposableInstance);
        }
    }

	public partial class Startup
	{
	    private void ConfigureIdentityServer(IAppBuilder app, IdentityServerConfiguration configuration)
	    {
	        LogProvider.SetCurrentLogProvider(new TestLogger());

            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Id;

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

                    RequireSsl = false,
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
            return new X509Certificate2(string.Format(@"{0}\bin\DasIDPCert.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");

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