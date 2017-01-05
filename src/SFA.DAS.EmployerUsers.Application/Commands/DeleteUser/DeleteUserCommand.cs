using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.DeleteUser
{
    public class DeleteUserCommand : IAsyncRequest
    {
        public User User { get; set; }
    }
}
