using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.UpdateUser
{
    public class UpdateUserCommand : IAsyncRequest
    {
        public User User { get; set; }
    }
}
