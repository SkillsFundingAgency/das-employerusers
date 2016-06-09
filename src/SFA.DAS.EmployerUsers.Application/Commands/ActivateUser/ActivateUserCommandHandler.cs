using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ActivateUser
{
    public class ActivateUserCommandHandler : AsyncRequestHandler<ActivateUserCommand>
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IValidator<ActivateUserCommand> _activateUserCommandValidator;
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationService _communicationService;


        public ActivateUserCommandHandler(IValidator<ActivateUserCommand> activateUserCommandValidator, IUserRepository userRepository, ICommunicationService communicationService)
        {
            _activateUserCommandValidator = activateUserCommandValidator;
            _userRepository = userRepository;
            _communicationService = communicationService;
        }

        protected override async Task HandleCore(ActivateUserCommand message)
        {
            Logger.Debug($"Received ActivateUserCommand for userId '{message.UserId}', Email Address '{message.Email}' with access code '{message.AccessCode}'");

            var user = (!string.IsNullOrEmpty(message.Email) && string.IsNullOrEmpty(message.UserId) && string.IsNullOrEmpty(message.AccessCode)) 
                            ? await _userRepository.GetByEmailAddress(message.Email) 
                            : await _userRepository.GetById(message.UserId);

            message.User = user;
            var result = _activateUserCommandValidator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            if (user.IsActive)
            {
                return;
            }

            user.IsActive = true;
            user.AccessCode = string.Empty;

            await _userRepository.Update(user);

            await _communicationService.SendUserAccountConfirmationMessage(user, Guid.NewGuid().ToString());
        }
    }
}
