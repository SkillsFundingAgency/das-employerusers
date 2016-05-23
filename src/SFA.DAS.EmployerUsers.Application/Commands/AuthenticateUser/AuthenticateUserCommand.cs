using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser
{
    public class AuthenticateUserCommand : IAsyncRequest<User>
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}
