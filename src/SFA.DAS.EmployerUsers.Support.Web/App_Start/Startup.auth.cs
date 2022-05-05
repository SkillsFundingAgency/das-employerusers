using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.SiteConnection;
using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Support.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            var ioc = DependencyResolver.Current;
            var siteValidatorSettings = ioc.GetService<ISiteValidatorSettings>();

            var logger = ioc.GetService<ILog>();
            logger.Info($"ConfigreAuth Audience:{siteValidatorSettings.Audience} Tenant:{siteValidatorSettings.Tenant}");

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
               new WindowsAzureActiveDirectoryBearerAuthenticationOptions
               {
                   TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidAudiences = siteValidatorSettings.Audience.Split(','),
                       RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                   },
                   Tenant = siteValidatorSettings.Tenant
               });
        }
    }
}   