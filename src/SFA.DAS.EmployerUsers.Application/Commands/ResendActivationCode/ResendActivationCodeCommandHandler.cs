using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode
{
    public class ResendActivationCodeCommandHandler : AsyncRequestHandler<ResendActivationCodeCommand>
    {
        private readonly ILogger _logger;

        private readonly IValidator<ResendActivationCodeCommand> _commandValidator;
        private readonly IUserRepository _userRepository;
        private readonly ICommunicationService _communicationService;
        private readonly ICodeGenerator _codeGenerator;

        public ResendActivationCodeCommandHandler(IValidator<ResendActivationCodeCommand> commandValidator, IUserRepository userRepository, ICommunicationService communicationService, ICodeGenerator codeGenerator, ILogger logger)
        {
            _commandValidator = commandValidator;
            _userRepository = userRepository;
            _communicationService = communicationService;
            _codeGenerator = codeGenerator;
            _logger = logger;
        }

        protected override async Task HandleCore(ResendActivationCodeCommand message)
        {
            _logger.Debug($"Received ResendActivationCodeCommand for user '{message.UserId}'");

            var validationResult = await _commandValidator.ValidateAsync(message);
            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var user = await _userRepository.GetById(message.UserId);

            if (user == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "UserNotFound", "User not found" } });

            if (!user.IsActive)
            {
                if (!user.SecurityCodes.Any(sc => sc.CodeType == Domain.SecurityCodeType.AccessCode && sc.ExpiryTime >= DateTime.Now))
                {
                    user.AddSecurityCode(new Domain.SecurityCode
                    {
                        Code = _codeGenerator.GenerateAlphaNumeric(),
                        CodeType = Domain.SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.Today.AddDays(8).AddSeconds(-1),
                        ReturnUrl = user.SecurityCodes.OrderByDescending(sc => sc.ExpiryTime).FirstOrDefault(sc => sc.CodeType == Domain.SecurityCodeType.AccessCode)?.ReturnUrl
                    });
                    await _userRepository.Update(user);
                }
                await _communicationService.ResendActivationCodeMessage(user, Guid.NewGuid().ToString());
            }

        }
    }
}