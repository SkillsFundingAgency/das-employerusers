using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Queries.IsPasswordResetValid
{
    public class IsPasswordResetCodeValidQueryHandler : IAsyncRequestHandler<IsPasswordResetCodeValidQuery,PasswordResetCodeResponse>
    {
        private readonly IUserRepository _userRepository;

        public IsPasswordResetCodeValidQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PasswordResetCodeResponse> Handle(IsPasswordResetCodeValidQuery message)
        {
            var passwordResetCodeResponse = new PasswordResetCodeResponse {IsValid = true};

            var user = await _userRepository.GetByEmailAddress(message.Email);

            if (user == null || !user.PasswordResetCode.Equals(message.PasswordResetCode, StringComparison.InvariantCultureIgnoreCase))
            {
                passwordResetCodeResponse.IsValid = false;
            }
            
            if(passwordResetCodeResponse.IsValid && user?.PasswordResetCodeExpiry != null && DateTime.UtcNow > user.PasswordResetCodeExpiry.Value)
            {
                passwordResetCodeResponse.IsValid = false;
                passwordResetCodeResponse.HasExpired = true;
            }
            
            return passwordResetCodeResponse;
        }
    }
}
