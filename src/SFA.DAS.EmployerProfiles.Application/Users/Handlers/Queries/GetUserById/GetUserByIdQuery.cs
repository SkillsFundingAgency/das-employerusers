using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<GetUserByIdQueryResult>
{
    public Guid Id { get; set; }
}