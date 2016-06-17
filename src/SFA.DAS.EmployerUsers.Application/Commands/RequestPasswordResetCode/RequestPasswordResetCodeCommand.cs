using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode
{
    public class RequestPasswordResetCodeCommand : IAsyncRequest
    {
        public string Email { get; set; }   
    }
}