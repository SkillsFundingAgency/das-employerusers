using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.ChangeEmail
{
    public class ChangeEmailCommand : IAsyncRequest<Unit>
    {
        public User User { get; set; }
        public string SecurityCode { get; set; }
        public string Password { get; set; }
    }
}
