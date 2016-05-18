using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Data.User
{
    public interface IUserRepository
    {
        Task<Domain.User> GetById(string id);
        void Create(Domain.User registerUser);
    }
}
