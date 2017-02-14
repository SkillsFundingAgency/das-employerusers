using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.SearchUsers
{
    public class SearchUsersQueryResponse
    {
        public User[] Users { get; set; }

        public int RecordCount { get; set; }
    }
}