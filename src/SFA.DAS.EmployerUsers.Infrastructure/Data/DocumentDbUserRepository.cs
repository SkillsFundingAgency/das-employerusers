using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using User = SFA.DAS.EmployerUsers.Domain.User;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public class DocumentDbUserRepository : IUserRepository
    {
        private const string DatabaseName = "EmployerUsers";
        private const string CollectionName = "User";

        private readonly IConfigurationService _configurationService;

        public DocumentDbUserRepository(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task<User> GetById(string id)
        {
            try
            {
                var client = await GetClient();

                var documentUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id);
                var document = await client.ReadDocumentAsync(documentUri);

                DocumentDbUser documentDbUser = (dynamic)document.Resource;
                return documentDbUser.ToDomainUser();
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public async Task Create(User registerUser)
        {
            var client = await GetClient();

            registerUser.Id = Guid.NewGuid().ToString();

            var collectionId = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var documentDbUser = DocumentDbUser.FromDomainUser(registerUser);
            await client.CreateDocumentAsync(collectionId, documentDbUser, null, true);
        }

        private async Task<DocumentClient> GetClient()
        {
            var configuration = await _configurationService.Get<EmployerUsersConfiguration>();
            return new DocumentClient(new Uri(configuration.DataStorage.DocumentDbUri), configuration.DataStorage.DocumentDbAccessToken);
        }

        public async Task Update(User user)
        {
            var client = await GetClient();

            var collectionId = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var documentDbUser = DocumentDbUser.FromDomainUser(user);
            await client.ReplaceDocumentAsync(collectionId, documentDbUser);
        }
    }
}
