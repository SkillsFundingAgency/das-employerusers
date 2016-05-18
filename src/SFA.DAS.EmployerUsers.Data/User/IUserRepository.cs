using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Data.User
{
    public interface IUserRepository
    {
        Task Create(Domain.User registerUser);
    }
}
