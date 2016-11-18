using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public class SqlServerRelyingPartyRepository : SqlServerRepository, IRelyingPartyRepository
    {
        public SqlServerRelyingPartyRepository()
            : base("UsersConnectionString")
        {
            
        }   
         
        public async Task<RelyingParty[]> GetAllAsync()
        {
            return await Query<RelyingParty>("GetAllRelyingParties");
        }
    }
}
