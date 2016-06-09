using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommandHandler : AsyncRequestHandler<UnlockUserCommand>
    {
        private readonly IValidator<UnlockUserCommand> _unlockUserCommandValidator;
        private readonly IUserRepository _userRepository;

        public UnlockUserCommandHandler(IValidator<UnlockUserCommand> unlockUserCommandValidator, IUserRepository userRepository)
        {
            _unlockUserCommandValidator = unlockUserCommandValidator;
            _userRepository = userRepository;
        }

        protected override async Task HandleCore(UnlockUserCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(typeof(UnlockUserCommand).Name,"Unlock User Command Is Null");
            }

            message.User = await _userRepository.GetByEmailAddress(message.Email);

            var result = _unlockUserCommandValidator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            message.User.FailedLoginAttempts = 0;
            message.User.IsLocked = false;

            await _userRepository.Update(message.User);
        }
    }
}
