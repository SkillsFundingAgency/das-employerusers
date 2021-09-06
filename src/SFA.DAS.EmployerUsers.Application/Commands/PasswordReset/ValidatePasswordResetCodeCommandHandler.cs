using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Extensions;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class ValidatePasswordResetCodeCommandHandler : IAsyncRequestHandler<ValidatePasswordResetCodeCommand, ValidatePasswordResetCodeResponse>
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<ValidatePasswordResetCodeCommand> _validator;

        public ValidatePasswordResetCodeCommandHandler(
            IUserRepository userRepository,
            IValidator<ValidatePasswordResetCodeCommand> validator,
            ILogger logger
        )
        {
            _userRepository = userRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<ValidatePasswordResetCodeResponse> Handle(ValidatePasswordResetCodeCommand message)
        {
            _logger.Info($"Received ValidatePasswordResetCodeCommand for email '{message.Email}'");

            var user = await _userRepository.GetByEmailAddress(message.Email);
            message.User = user;

            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                await ProcessFailedAttempt(user);
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            _logger.Info($"Validated PasswordResetCode for user id '{message.User.Id}'");
            
            var resetCode = message.User?.SecurityCodes?.MatchSecurityCode(message.PasswordResetCode);
            return new ValidatePasswordResetCodeResponse { ResetCode = resetCode };
        }

        private async Task ProcessFailedAttempt(User user)
        {
            var latestSecurityCode = user?.SecurityCodes?.LatestValidSecurityCode();
            
            if (latestSecurityCode != null)
            {
                latestSecurityCode.FailedAttempts += 1;
                await _userRepository.Update(user);

                if (latestSecurityCode.FailedAttempts >= 3)
                {
                    throw new ExceededLimitPasswordResetCodeException("Too many failed attempts, reset code has expired");
                }
            }
        }
    }

    public class ValidatePasswordResetCodeResponse
    {
        public SecurityCode ResetCode { get; set; }
    }
}
