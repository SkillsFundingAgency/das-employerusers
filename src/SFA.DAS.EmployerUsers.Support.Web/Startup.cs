using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerUsers.Support.Web.Startup))]

namespace SFA.DAS.EmployerUsers.Support.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}