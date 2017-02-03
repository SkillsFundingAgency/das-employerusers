using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Queries.SearchUsers
{
    public class SearchUsersQuery : IAsyncRequest<SearchUsersQueryResponse>
    {
        public string Criteria { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
