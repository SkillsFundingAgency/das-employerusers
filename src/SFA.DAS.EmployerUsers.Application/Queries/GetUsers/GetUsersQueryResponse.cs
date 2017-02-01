using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUsers
{
    public class GetUsersQueryResponse
    {
        public User[] Users { get; set; }
        
        public int RecordCount { get; set; }
    }
}