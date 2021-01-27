using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResumeUser
{
    public class ResumeUserCommand : IAsyncRequest
    {
        public User User { get; set; }
    }
}
