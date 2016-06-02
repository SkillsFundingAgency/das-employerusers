using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode
{
    public class ResendActivationCodeCommand : IAsyncRequest
    {
        public string UserId { get; set; }
    }
}