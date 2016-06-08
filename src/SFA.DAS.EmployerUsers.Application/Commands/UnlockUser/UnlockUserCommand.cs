using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommand : IAsyncRequest
    {
        public string UnlockCode { get; set; }
        public string Email { get; set; }
    }
}
