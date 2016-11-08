using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommandHandler : AsyncRequestHandler<UnlockUserCommand>
    {
        private readonly IValidator<UnlockUserCommand> _unlockUserCommandValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ICommunicationService _communicationService;

        public UnlockUserCommandHandler(IValidator<UnlockUserCommand> unlockUserCommandValidator, IUserRepository userRepository, IMediator mediator, ICommunicationService communicationService)
        {
            _unlockUserCommandValidator = unlockUserCommandValidator;
            _userRepository = userRepository;
            _mediator = mediator;
            _communicationService = communicationService;
        }

        protected override async Task HandleCore(UnlockUserCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(typeof(UnlockUserCommand).Name, "Unlock User Command Is Null");
            }

            message.User = await _userRepository.GetByEmailAddress(message.Email);

            if (message.User != null && !message.User.IsLocked)
            {
                return;
            }

            var result = _unlockUserCommandValidator.Validate(message);

            if (!result.IsValid())
            {
                if (message.User != null)
                {
                    await _mediator.PublishAsync(new AccountLockedEvent { User = message.User });
                }
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            message.User.FailedLoginAttempts = 0;
            message.User.IsLocked = false;
            message.User.ExpireSecurityCodesOfType(Domain.SecurityCodeType.UnlockCode);


            await _userRepository.Update(message.User);

            await _communicationService.SendUserUnlockedMessage(message.User, Guid.NewGuid().ToString());
        }
    }
}
