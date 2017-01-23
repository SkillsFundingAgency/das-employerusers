using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode
{
    public class ResendUnlockCodeCommand : IAsyncRequest
    {
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
    }
}
