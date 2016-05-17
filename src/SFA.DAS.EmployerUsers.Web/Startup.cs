using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerUsers.Web.Startup))]
namespace SFA.DAS.EmployerUsers.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureIdentityServer(app);
            ConfigureRelyingParty(app);
        }
    }
}