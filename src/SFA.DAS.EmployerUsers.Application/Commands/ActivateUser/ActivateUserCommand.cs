using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.ActivateUser
{
    public class ActivateUserCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public string AccessCode { get; set; }
        public User User { get; set; }
    }
}
