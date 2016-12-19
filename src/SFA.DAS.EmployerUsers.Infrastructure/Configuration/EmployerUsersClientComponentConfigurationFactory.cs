using NLog;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class EmployerUsersClientComponentConfigurationFactory : ConfigurationFactory
    {
        private readonly ILogger _logger;

        private readonly IConfigurationService _configurationService;

        public EmployerUsersClientComponentConfigurationFactory(IConfigurationService configurationService, ILogger logger)
        {
            _configurationService = configurationService;
            _logger = logger;
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
