using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class ValidatePasswordResetCodeCommand : IAsyncRequest<ValidatePasswordResetCodeResponse>
    {
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }
        public User User { get; set; }
    }
}
