using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Queries.IsPasswordResetValid
{
    public class IsPasswordResetCodeValidQueryHandler : IAsyncRequestHandler<IsPasswordResetCodeValidQuery, PasswordResetCodeResponse>
    {
        private readonly IUserRepository _userRepository;

        public IsPasswordResetCodeValidQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PasswordResetCodeResponse> Handle(IsPasswordResetCodeValidQuery message)
        {
            var passwordResetCodeResponse = new PasswordResetCodeResponse { IsValid = true };

            var user = await _userRepository.GetByEmailAddress(message.Email);
            var resetCode = user?.SecurityCodes?.OrderBy(sc => sc.ExpiryTime).FirstOrDefault(sc => sc.Code.Equals(message.PasswordResetCode, StringComparison.InvariantCultureIgnoreCase) && sc.CodeType == Domain.SecurityCodeType.PasswordResetCode);

            if (resetCode == null)
            {
                passwordResetCodeResponse.IsValid = false;
            }
            else if (resetCode.ExpiryTime < DateTime.UtcNow)
            {
                passwordResetCodeResponse.IsValid = false;
                passwordResetCodeResponse.HasExpired = true;
            }

            return passwordResetCodeResponse;
        }
    }
}
