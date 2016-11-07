using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Domain.Data
{
    public interface IUserRepository : IRepository
    {
        Task<User> GetById(string id);
        Task<User> GetByEmailAddress(string emailAddress);
        Task Create(User registerUser);
        Task Update(User user);
    }
}
