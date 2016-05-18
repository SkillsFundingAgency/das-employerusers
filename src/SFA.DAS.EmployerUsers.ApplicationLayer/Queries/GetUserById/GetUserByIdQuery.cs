using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.Queries.GetUserById
{
    public class GetUserByIdQuery : IAsyncRequest<User>
    {
        public string UserId { get; set; }
    }
}
