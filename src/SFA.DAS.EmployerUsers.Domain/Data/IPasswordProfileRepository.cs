using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Domain.Data
{
    public interface IPasswordProfileRepository
    {
        Task<IEnumerable<PasswordProfile>> GetAllAsync();
    }
}
