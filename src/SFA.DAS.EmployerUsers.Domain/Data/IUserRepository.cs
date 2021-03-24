using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Domain.Data
{
    public interface IUserRepository : IRepository
    {
        Task<User> GetById(string id);
        Task<User> GetByEmailAddress(string emailAddress);
        Task<User[]> GetUsersWithExpiredRegistrations();
        Task<User[]> GetUsers(int pageSize, int pageNumber);
        Task Create(User registerUser);
        Task Update(User user);
        Task Delete(User user);
        Task<int> GetUserCount();
        Task<Users> SearchUsers(string criteria, int pageSize, int pageNumber);

        Task Suspend(User user);
        Task Resume(User user);
    }
}
