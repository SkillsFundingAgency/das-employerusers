using Microsoft.Owin;
using Owin;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerUsers.Web.Startup))]
namespace SFA.DAS.EmployerUsers.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configurationService = StructuremapMvc.Container.GetInstance<IConfigurationService>();
            configurationService.Get<EmployerUsersConfiguration>().ContinueWith((task) =>
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }

                var configuration = task.Result;
                ConfigureIdentityServer(app, configuration.IdentityServer);
                ConfigureRelyingParty(app);
            }).Wait();
        }
    }
}