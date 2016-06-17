using MediatR;

namespace SFA.DAS.EmployerUsers.Application.Queries.IsPasswordResetValid
{
    public class IsPasswordResetCodeValidQuery : IAsyncRequest<PasswordResetCodeResponse>
    {
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }
    }
}
