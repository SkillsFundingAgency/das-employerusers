using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Support.Shared.SiteConnection;

[assembly: OwinStartup(typeof(SFA.DAS.EmployerUsers.Support.Web.Startup))]

namespace SFA.DAS.EmployerUsers.Support.Web
{
    public class Startup
    {
        private readonly ILog _logger;
        private readonly ISiteValidatorSettings _siteValidatorSettings;

        public Startup(ILog logger, ISiteValidatorSettings siteValidatorSettings)
        {
            _logger = logger;
            _siteValidatorSettings = siteValidatorSettings;
        }

        public void Configuration(IAppBuilder app)
        {
            _logger.Info($"Configure Authentication Audience:{_siteValidatorSettings.Audience} Tenant:{_siteValidatorSettings.Tenant}");

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
               new WindowsAzureActiveDirectoryBearerAuthenticationOptions
               {
                   TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidAudiences = _siteValidatorSettings.Audience.Split(','),
                       RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                   },
                   Tenant = _siteValidatorSettings.Tenant
               });
        }
    }
}