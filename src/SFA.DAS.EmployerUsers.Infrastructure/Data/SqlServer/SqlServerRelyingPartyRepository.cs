using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Infrastructure.Data.SqlServer
{
    public class SqlServerRelyingPartyRepository : IRelyingPartyRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public SqlServerRelyingPartyRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }   
         
        public async Task<RelyingParty[]> GetAllAsync()
        {
            return await _unitOfWork.Query<RelyingParty>("GetAllRelyingParties");
        }
    }
}
