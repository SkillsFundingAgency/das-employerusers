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
                       ValidAudience = CloudConfigurationManager.GetSetting("idaAudience"),
                       RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                   },
                   Tenant = CloudConfigurationManager.GetSetting("idaTenant")
               });
        }
    }
}