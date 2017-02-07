using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerUsers.Api.Startup))]

namespace SFA.DAS.EmployerUsers.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}