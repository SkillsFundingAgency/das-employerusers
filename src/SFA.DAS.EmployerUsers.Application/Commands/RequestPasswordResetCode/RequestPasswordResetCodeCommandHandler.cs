using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode
{
    public class RequestPasswordResetCodeCommandHandler : AsyncRequestHandler<RequestPasswordResetCodeCommand>
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IValidator<RequestPasswordResetCodeCommand> _validator;
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationService _communicationService;
        private readonly ICodeGenerator _codeGenerator;

        public RequestPasswordResetCodeCommandHandler(IValidator<RequestPasswordResetCodeCommand> validator, IUserRepository userRepository, ICommunicationService communicationService, ICodeGenerator codeGenerator)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (userRepository == null)
                throw new ArgumentNullException(nameof(userRepository));
            if (communicationService == null)
                throw new ArgumentNullException(nameof(communicationService));
            if (codeGenerator == null)
                throw new ArgumentNullException(nameof(codeGenerator));
            _validator = validator;
            _userRepository = userRepository;
            _communicationService = communicationService;
            _codeGenerator = codeGenerator;
        }

        protected override async Task HandleCore(RequestPasswordResetCodeCommand message)
        {
            Logger.Debug($"Received RequestPasswordResetCodeCommand for user '{message.Email}'");

            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var existingUser = await _userRepository.GetByEmailAddress(message.Email);

            if (existingUser == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "UserNotFound", $"User '{message.Email}' not found" } });

            if (ExistingUserHasActivePasswordResetCode(existingUser))
            {
                //TODO: Resend message
            }
            else
            {
                existingUser.PasswordResetCode = _codeGenerator.GenerateAlphaNumeric();
                existingUser.PasswordResetCodeExpiry = DateTimeProvider.Current.UtcNow.AddDays(1);

                await _userRepository.Update(existingUser);
                //TODO: Send message
            }
        }

        private static bool ExistingUserHasActivePasswordResetCode(User user)
        {
            if (string.IsNullOrEmpty(user.PasswordResetCode) || !user.PasswordResetCodeExpiry.HasValue)
                return false;

            return user.PasswordResetCodeExpiry.Value < DateTimeProvider.Current.UtcNow;
        }
    }
}