using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Domain.Links;
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
        private readonly ILinkBuilder _linkBuilder;

        public RequestPasswordResetCodeCommandHandler(IValidator<RequestPasswordResetCodeCommand> validator,
                                                      IUserRepository userRepository,
                                                      ICommunicationService communicationService,
                                                      ICodeGenerator codeGenerator,
                                                      ILinkBuilder linkBuilder)
        {
            if (validator == null)
                throw new ArgumentNullException(nameof(validator));
            if (userRepository == null)
                throw new ArgumentNullException(nameof(userRepository));
            if (communicationService == null)
                throw new ArgumentNullException(nameof(communicationService));
            if (codeGenerator == null)
                throw new ArgumentNullException(nameof(codeGenerator));
            if (linkBuilder == null)
                throw new ArgumentNullException(nameof(linkBuilder));

            _validator = validator;
            _userRepository = userRepository;
            _communicationService = communicationService;
            _codeGenerator = codeGenerator;
            _linkBuilder = linkBuilder;
        }

        protected override async Task HandleCore(RequestPasswordResetCodeCommand message)
        {
            Logger.Debug($"Received RequestPasswordResetCodeCommand for user '{message.Email}'");

            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var existingUser = await _userRepository.GetByEmailAddress(message.Email);

            if (existingUser == null)
            {
                await _communicationService.SendNoAccountToPasswordResetMessage(message.Email, Guid.NewGuid().ToString(), _linkBuilder.GetRegistrationUrl());
                return;
            }

            if (RequiresPasswordResetCode(existingUser))
            {
                existingUser.AddSecurityCode(new SecurityCode
                {
                    Code = _codeGenerator.GenerateAlphaNumeric(),
                    CodeType = SecurityCodeType.PasswordResetCode,
                    ExpiryTime = DateTimeProvider.Current.UtcNow.AddDays(1)
                });

                await _userRepository.Update(existingUser);
            }

            await _communicationService.SendPasswordResetCodeMessage(existingUser, Guid.NewGuid().ToString());
        }

        private static bool RequiresPasswordResetCode(User user)
        {
            return !user.SecurityCodes.Any(sc => sc.CodeType == SecurityCodeType.PasswordResetCode
                                             && sc.ExpiryTime >= DateTime.UtcNow);
        }
    }
}