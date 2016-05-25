using Microsoft.Owin;
using NLog;
using Owin;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerUsers.Web.Startup))]
namespace SFA.DAS.EmployerUsers.Web
{
    public partial class Startup
    {
        protected static readonly Logger _logger = LogManager.GetLogger("Startup");

        public void Configuration(IAppBuilder app)
        {
            _logger.Debug("Started running Owin Configuration");

            var configurationService = StructuremapMvc.Container.GetInstance<IConfigurationService>();
            configurationService.Get<EmployerUsersConfiguration>().ContinueWith((task) =>
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }

                _logger.Debug("EmployerUsersConfiguration read successfully");

                var configuration = task.Result;
                ConfigureIdentityServer(app, configuration.IdentityServer);
                ConfigureRelyingParty(app, configuration.IdentityServer);
            }).Wait();
        }
    }
}