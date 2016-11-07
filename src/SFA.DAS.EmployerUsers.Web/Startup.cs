using Microsoft.Owin;
using NLog;
using Owin;
using SFA.DAS.Configuration;
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

            var configurationService = StructuremapMvc.Container.GetInstance<IConfigurationService>();
            var relyingPartyRepository = StructuremapMvc.Container.GetInstance<IRelyingPartyRepository>();
            configurationService.GetAsync<EmployerUsersConfiguration>().ContinueWith((task) =>
            {
                if (task.Exception != null)
                {
                    task.Exception.UnpackAndLog(_logger);
                    throw task.Exception.InnerExceptions[0];
                }

                _logger.Debug("EmployerUsersConfiguration read successfully");

                var configuration = task.Result;
                ConfigureIdentityServer(app, configuration.IdentityServer, relyingPartyRepository);
                ConfigureRelyingParty(app, configuration.IdentityServer);
            }).Wait();
        }
    }
}