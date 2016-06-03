using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode
{
    public class ResendActivationCodeCommandHandler : AsyncRequestHandler<ResendActivationCodeCommand>
    {
        private readonly IValidator<ResendActivationCodeCommand> _commandValidator;
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationService _communicationService;

        public ResendActivationCodeCommandHandler(IValidator<ResendActivationCodeCommand> commandValidator, IUserRepository userRepository, ICommunicationService communicationService)
        {
            if (commandValidator == null)
                throw new ArgumentNullException(nameof(commandValidator));
            if (userRepository == null)
                throw new ArgumentNullException(nameof(userRepository));
            if (communicationService == null)
                throw new ArgumentNullException(nameof(communicationService));
            _commandValidator = commandValidator;
            _userRepository = userRepository;
            _communicationService = communicationService;
        }

        protected override async Task HandleCore(ResendActivationCodeCommand message)
        {
            if (_commandValidator.Validate(message).Any())
                throw new InvalidRequestException(new[] { "NotValid" });

            var user = await _userRepository.GetById(message.UserId);

            if (user == null)
                throw new InvalidRequestException(new[] { "UserNotFound" });

            if (!user.IsActive)
            {
                await _communicationService.ResendActivationCodeMessage(user, Guid.NewGuid().ToString());
            }
        }
    }
}