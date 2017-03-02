using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Commands.ForcePasswordReset
{
    public class ForcePasswordResetCommand : IAsyncRequest
    {
        public string UserId { get; set; }
    }
}
