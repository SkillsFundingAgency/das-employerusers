using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Validation;
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

            var result = _unlockUserCommandValidator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var user = await _userRepository.GetByEmailAddress(message.Email);

            if (user == null)
            {
                return;
            }

            user.FailedLoginAttempts = 0;
            user.IsLocked = false;

            await _userRepository.Update(user);
        }
    }
}
