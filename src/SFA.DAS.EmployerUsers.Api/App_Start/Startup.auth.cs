using System.Configuration;
using Microsoft.Azure;
using Owin;
using Microsoft.Owin.Security.ActiveDirectory;

namespace SFA.DAS.EmployerUsers.Api
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
               new WindowsAzureActiveDirectoryBearerAuthenticationOptions
               {
                   TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidAudiences = ConfigurationManager.AppSettings["idaAudience"].Split(','),
                       RoleClaimType = "roles"
                   },
                   Tenant = ConfigurationManager.AppSettings["idaTenant"]
               });
        }
    }
}   