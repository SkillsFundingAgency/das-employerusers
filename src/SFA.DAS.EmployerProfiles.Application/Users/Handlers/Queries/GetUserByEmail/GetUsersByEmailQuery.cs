using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Queries.GetUserByEmail;

public class GetUsersByEmailQuery : IRequest<GetUsersByEmailQueryResult>
{
    public string Email { get; set; }
}