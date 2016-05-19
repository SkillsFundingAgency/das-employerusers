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

        private readonly DocumentClient _client;

        public DocumentDbUserRepository(IConfigurationService configurationService)
        {
            var cfgTask = configurationService.Get<EmployerUsersConfiguration>();
            cfgTask.Wait();
            var configuration = cfgTask.Result;

            _client = new DocumentClient(new Uri(configuration.DataStorage.DocumentDbUri), configuration.DataStorage.DocumentDbAccessToken);
        }

        public async Task<User> GetById(string id)
        {
            try
            {
                var documentUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, id);
                var document = await _client.ReadDocumentAsync(documentUri);

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
            registerUser.Id = Guid.NewGuid().ToString();

            var collectionId = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var documentDbUser = DocumentDbUser.FromDomainUser(registerUser);
            await _client.CreateDocumentAsync(collectionId, documentDbUser, null, true);
        }
    }
}
