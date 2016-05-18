using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Domain.Data
{
    public interface IUserRepository : IRepository
    {
        Task<Domain.User> GetById(string id);
        Task Create(Domain.User registerUser);
    }
}
