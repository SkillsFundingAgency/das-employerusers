using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Registration;
using SFA.DAS.EmployerUsers.Domain.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application.Commands.ActivateUser
{
    public class ActivateUserCommandHandler : IAsyncRequestHandler<ActivateUserCommand, ActivateUserCommandResult>
    {
        private readonly IAuditService _auditService;
        private readonly ILogger _logger;
        private readonly IValidator<ActivateUserCommand> _activateUserCommandValidator;
        private readonly IUserRepository _userRepository;

        public ActivateUserCommandHandler(IValidator<ActivateUserCommand> activateUserCommandValidator, IUserRepository userRepository, IAuditService auditService, ILogger logger)
        {
            _activateUserCommandValidator = activateUserCommandValidator;
            _userRepository = userRepository;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ActivateUserCommandResult> Handle(ActivateUserCommand message)
        {
            _logger.Debug($"Received ActivateUserCommand for userId '{message.UserId}' with access code '{message.AccessCode}'");

            var user = (!string.IsNullOrEmpty(message.Email) && string.IsNullOrEmpty(message.UserId) && string.IsNullOrEmpty(message.AccessCode))
                            ? await _userRepository.GetByEmailAddress(message.Email)
                            : await _userRepository.GetById(message.UserId);

            message.User = user;
            var validationResult = await _activateUserCommandValidator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                await _auditService.WriteAudit(new FailedActivationAuditMessage(user, message.AccessCode));
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }


            var securityCode = message.User.SecurityCodes?.SingleOrDefault(sc => sc.Code.Equals(message.AccessCode, StringComparison.CurrentCultureIgnoreCase)
                                                                    && sc.CodeType == Domain.SecurityCodeType.AccessCode);



            var result = new ActivateUserCommandResult
            {
                ReturnUrl = securityCode?.ReturnUrl
            };


            if (user.IsActive)
            {
                return result;
            }

            user.IsActive = true;
            user.ExpireSecurityCodesOfType(Domain.SecurityCodeType.AccessCode);
            await _userRepository.Update(user);

            await _auditService.WriteAudit(new ActivatedAuditMessage(user));

            return result;
        }
    }
}
