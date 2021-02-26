using System;
using System.Net;
using Microsoft.Owin;
using NLog;
using Owin;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerUsers.Web.Startup))]
namespace SFA.DAS.EmployerUsers.Web
{
    public partial class Startup
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void Configuration(IAppBuilder app)
        {
            _logger.Debug("Started running Owin Configuration");
            
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var identityServerConfiguration = StructuremapMvc.Container.GetInstance<IdentityServerConfiguration>();
            var relyingPartyRepository = StructuremapMvc.Container.GetInstance<IRelyingPartyRepository>();

                try
                {
                    ConfigureIdentityServer(app, identityServerConfiguration, relyingPartyRepository);
                    ConfigureRelyingParty(app, identityServerConfiguration);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error in startup - {ex.Message}");
                }
        }
    }
}