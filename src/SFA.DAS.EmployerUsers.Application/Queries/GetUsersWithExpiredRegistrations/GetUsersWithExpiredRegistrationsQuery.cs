using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Queries.GetUsersWithExpiredRegistrations
{
    public class GetUsersWithExpiredRegistrationsQuery : IAsyncRequest<User[]>
    {
    }
}
