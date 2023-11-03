using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Support.Application.Handlers
{
    public interface IEmployerUserHandler
    {
        Task<IEnumerable<UserSearchModel>> FindSearchItems(int pagesize, int pageNumber);

        Task<int> TotalUserRecords(int pagesize);
        
    }
}