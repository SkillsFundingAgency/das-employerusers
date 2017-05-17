using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommand : IAsyncRequest<UnlockUserResponse>
    {
        public string UnlockCode { get; set; }
        public string Email { get; set; }
        public User User { get; set; }
        public string ReturnUrl { get; set; }
    }
}
