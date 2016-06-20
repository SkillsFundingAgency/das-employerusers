using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class PasswordResetCommandHandler : AsyncRequestHandler<PasswordResetCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<PasswordResetCommand> _validator;
        private readonly ICommunicationService _communicationService;

        public PasswordResetCommandHandler(IUserRepository userRepository, IValidator<PasswordResetCommand> validator, ICommunicationService communicationService)
        {
            _userRepository = userRepository;
            _validator = validator;
            _communicationService = communicationService;
        }

        protected override async Task HandleCore(PasswordResetCommand message)
        {
            var user = await _userRepository.GetByEmailAddress(message.Email);
            message.User = user;

            var validationResult = _validator.Validate(message);
            
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            message.User.PasswordResetCode = string.Empty;
            message.User.PasswordResetCodeExpiry = null;
            message.User.Password = message.Password;

            await _userRepository.Update(message.User);

            await _communicationService.SendPasswordResetConfirmationMessage(user, Guid.NewGuid().ToString());
        }
    }
}
