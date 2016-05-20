using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUserById
{
    public class GetUserByIdQuery : IAsyncRequest<User>
    {
        public string UserId { get; set; }
    }
}
