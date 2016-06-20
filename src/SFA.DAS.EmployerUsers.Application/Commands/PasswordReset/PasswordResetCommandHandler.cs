using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class PasswordResetCommandHandler : AsyncRequestHandler<PasswordResetCommand>
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IUserRepository _userRepository;
        private readonly IValidator<PasswordResetCommand> _validator;
        private readonly ICommunicationService _communicationService;
        private readonly IPasswordService _passwordService;

        public PasswordResetCommandHandler(IUserRepository userRepository, IValidator<PasswordResetCommand> validator, ICommunicationService communicationService, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _validator = validator;
            _communicationService = communicationService;
            _passwordService = passwordService;
        }

        protected override async Task HandleCore(PasswordResetCommand message)
        {
            var sendPasswordResetConfirmationMessage = false;
            Logger.Info($"Received PasswordResetCommand for user '{message.Email}'");

            var user = await _userRepository.GetByEmailAddress(message.Email);
            message.User = user;

            var validationResult = _validator.Validate(message);
            
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var securedPassword = await _passwordService.GenerateAsync(message.Password);


            if (!message.User.IsActive)
            {
                sendPasswordResetConfirmationMessage = true;
            }

            message.User.PasswordResetCode = string.Empty;
            message.User.PasswordResetCodeExpiry = null;
            message.User.Password = securedPassword.HashedPassword;
            message.User.PasswordProfileId = securedPassword.ProfileId;
            message.User.Salt = securedPassword.Salt;
            message.User.IsActive = true;
            message.User.AccessCode = string.Empty;

            Logger.Info($"Password changed for user '{message.Email}'");

            await _userRepository.Update(message.User);

            
            await _communicationService.SendPasswordResetConfirmationMessage(user, Guid.NewGuid().ToString());

            if (sendPasswordResetConfirmationMessage)
            {
                await _communicationService.SendUserAccountConfirmationMessage(user, Guid.NewGuid().ToString());
            }
            
        }
    }
}
