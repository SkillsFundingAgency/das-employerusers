using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Api.Client
{
    public interface IEmployerUsersApiClient
    {
        Task<T> GetResource<T>(string resourceUri) where T : IEmployerUsersResource;

        Task<PagedApiResponseViewModel<UserSummaryViewModel>> GetPageOfEmployerUsers(int pageNumber = 1, int pageSize = 1000);

        Task<PagedApiResponseViewModel<UserSummaryViewModel>> SearchEmployerUsers(string criteria, int pageNumber = 1, int pageSize = 1000);
    }
}
