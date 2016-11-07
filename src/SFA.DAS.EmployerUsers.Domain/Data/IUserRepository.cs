using System;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Domain.Data
{
    public interface IUserRepository : IRepository
    {
        Task<Domain.User> GetById(string id);
        Task<Domain.User> GetByEmailAddress(string emailAddress);
        Task Create(User registerUser);
        Task Update(User user);

        Task StoreSecurityCode(User user, string code, SecurityCodeType codeType, DateTime expiryTime);
        Task ExpirySecurityCodes(User user, SecurityCodeType codeType);
    }
}
