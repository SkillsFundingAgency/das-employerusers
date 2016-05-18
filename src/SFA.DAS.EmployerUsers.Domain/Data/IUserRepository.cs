using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Domain.Data
{
    public interface IUserRepository
    {
        Task<Domain.User> GetById(string id);
        void Create(Domain.User registerUser);
    }
}
