using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Domain.Data
{
    public interface IRelyingPartyRepository
    {
        Task<RelyingParty[]> GetAllAsync();
    }
}
