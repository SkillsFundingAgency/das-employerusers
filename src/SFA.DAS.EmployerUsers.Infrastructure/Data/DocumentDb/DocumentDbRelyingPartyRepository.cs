using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.DocumentDb
{
    public class DocumentDbRelyingPartyRepository : DocumentDbRepository, IRelyingPartyRepository
    {
        private const string DatabaseName = "EmployerUsers";
        private const string CollectionName = "RelyingParty";


        public DocumentDbRelyingPartyRepository(IConfigurationService configurationService)
            : base(configurationService)
        {
        }

        public async Task<RelyingParty[]> GetAllAsync()
        {
            var client = await GetClient();
            var collectionId = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var query = client.CreateDocumentQuery<DocumentDbRelyingParty>(collectionId, new FeedOptions());
            var results = query.ToArray();
            return results.Select(rp => rp.ToDomainRelyingParty()).ToArray();
        }
    }
}
