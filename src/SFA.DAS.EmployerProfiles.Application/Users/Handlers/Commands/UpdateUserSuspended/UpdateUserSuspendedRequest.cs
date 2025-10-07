using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerProfiles.Application.Users.Handlers.Commands.UpdateUserSuspended;

public class UpdateUserSuspendedRequest : IRequest<UpdateUserSuspendedResult>
{
    public Guid Id { get; set; }
    public bool UserSuspended { get; set; }
}