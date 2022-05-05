using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
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

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
               new WindowsAzureActiveDirectoryBearerAuthenticationOptions
               {
                   TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidAudiences = siteValidatorSettings.Audience.Split(','),
                       RoleClaimType = "roles"
                   },
                   Tenant = siteValidatorSettings.Tenant
               });
        }
    }
}   