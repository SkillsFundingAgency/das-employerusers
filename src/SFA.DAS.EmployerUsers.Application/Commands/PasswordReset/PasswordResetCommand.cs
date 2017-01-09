using MediatR;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class PasswordResetCommand : IAsyncRequest<PasswordResetResponse>
    {
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public User User { get; set; }
    }
}
