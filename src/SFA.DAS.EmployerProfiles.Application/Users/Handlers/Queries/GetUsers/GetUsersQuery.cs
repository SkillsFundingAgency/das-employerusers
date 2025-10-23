using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUsers;

public class GetUsersQuery : IRequest<GetUsersQueryResult>
{
    public int PageSize { get; set; } = 1000;
    public int PageNumber { get; set; } = 1;
}
