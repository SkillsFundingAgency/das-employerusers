using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.SuspendUser
{
    public class SuspendUserCommand : IAsyncRequest
    {
        public User User { get; set; }
    }
}
