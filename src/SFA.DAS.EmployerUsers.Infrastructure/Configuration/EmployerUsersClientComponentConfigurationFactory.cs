using NLog;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class EmployerUsersClientComponentConfigurationFactory : ConfigurationFactory
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;

        public EmployerUsersClientComponentConfigurationFactory(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public override ConfigurationContext Get()
        {
            _logger.Debug("Start reading configuration");

            var usersConfig = _configurationService.Get<EmployerUsersConfiguration>();

            _logger.Debug($"Got configuration. BaseUrl = {usersConfig.IdentityServer.ApplicationBaseUrl}");

            return new ConfigurationContext
            {
                AccountActivationUrl = usersConfig.IdentityServer.ApplicationBaseUrl + "account/confirm/"
            };
        }
    }
}
