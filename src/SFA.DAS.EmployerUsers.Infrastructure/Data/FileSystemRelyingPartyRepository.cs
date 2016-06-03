using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data
{
    public class FileSystemRelyingPartyRepository : FileSystemRepository, IRelyingPartyRepository
    {

        public FileSystemRelyingPartyRepository()
            : base("RelyingParties")
        {
        }

        public async Task<RelyingParty[]> GetAllAsync()
        {
            var files = GetDataFiles();
            var relyingParties = new List<RelyingParty>();
            foreach (var path in files)
            {
                relyingParties.Add(await ReadFile<RelyingParty>(path));
            }
            return relyingParties.ToArray();
        }
    }
}
