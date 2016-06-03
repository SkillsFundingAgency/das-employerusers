using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public abstract class DocumentDbRepository
    {
        protected DocumentDbRepository(IConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
        }

        protected IConfigurationService ConfigurationService { get; private set; }

        protected async Task<DocumentClient> GetClient()
        {
            var configuration = await ConfigurationService.GetAsync<EmployerUsersConfiguration>();
            return new DocumentClient(new Uri(configuration.DataStorage.DocumentDbUri), configuration.DataStorage.DocumentDbAccessToken);
        }
    }
}
