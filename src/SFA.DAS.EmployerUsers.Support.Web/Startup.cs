using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.SiteConnection;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerUsers.Support.Web.Startup))]

namespace SFA.DAS.EmployerUsers.Support.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var logger = DependencyResolver.Current.GetService<ILog>();
            var siteValidatorSettings = DependencyResolver.Current.GetService<ISiteValidatorSettings>();
            
            logger.Info($"Configure Authentication Audience:{siteValidatorSettings.Audience} Tenant:{siteValidatorSettings.Tenant}");

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