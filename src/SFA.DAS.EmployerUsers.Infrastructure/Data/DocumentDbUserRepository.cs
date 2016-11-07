using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using User = SFA.DAS.EmployerUsers.Domain.User;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public class DocumentDbUserRepository : DocumentDbRepository, IUserRepository
    {
        private const string DatabaseName = "EmployerUsers";
        private const string CollectionName = "User";

        public DocumentDbUserRepository(IConfigurationService configurationService)
            : base(configurationService)
        {
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
        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            var client = await GetClient();
            var collectionId = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var query = client.CreateDocumentQuery<DocumentDbUser>(collectionId)
                .Where(u => u.Email.ToLower() == emailAddress.ToLower()).AsDocumentQuery();
            
            var results = await query.ExecuteNextAsync<DocumentDbUser>();
            var user = results.SingleOrDefault();

            return user?.ToDomainUser();
            
        }
        public async Task Create(User registerUser)
        {
            var client = await GetClient();

            var collectionId = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var documentDbUser = DocumentDbUser.FromDomainUser(registerUser);
            
            await client.CreateDocumentAsync(collectionId, documentDbUser, null, true);
            
            
        }
        public async Task Update(User user)
        {
            var client = await GetClient();
            var documentUri = UriFactory.CreateDocumentUri(DatabaseName, CollectionName, user.Id);
            var documentDbUser = DocumentDbUser.FromDomainUser(user);
            await client.ReplaceDocumentAsync(documentUri, documentDbUser);
        }

        public Task StoreSecurityCode(User user, string code, SecurityCodeType codeType, DateTime expiryTime)
        {
            throw new NotImplementedException();
        }

        public Task ExpirySecurityCodes(User user, SecurityCodeType codeType)
        {
            throw new NotImplementedException();
        }
    }
}
