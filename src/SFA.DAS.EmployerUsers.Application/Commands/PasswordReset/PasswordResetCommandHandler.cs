using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Extensions;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Login;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class PasswordResetCommandHandler : IAsyncRequestHandler<PasswordResetCommand, PasswordResetResponse>
    {
        private readonly ILogger _logger;
        private readonly IAuditService _auditService;
        private readonly IUserRepository _userRepository;
        private readonly IValidator<PasswordResetCommand> _validator;
        private readonly ICommunicationService _communicationService;
        private readonly IPasswordService _passwordService;

        public PasswordResetCommandHandler(
            IUserRepository userRepository,
            IValidator<PasswordResetCommand> validator,
            ICommunicationService communicationService,
            IPasswordService passwordService,
            ILogger logger,
            IAuditService auditService)
        {
            _userRepository = userRepository;
            _validator = validator;
            _communicationService = communicationService;
            _passwordService = passwordService;
            _logger = logger;
            _auditService = auditService;
        }

        public async Task<PasswordResetResponse> Handle(PasswordResetCommand message)
        {
            _logger.Info($"Received PasswordResetCommand for email '{message.Email}'");

            var user = await _userRepository.GetByEmailAddress(message.Email);
            message.User = user;

            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {   
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var resetCode = message.User?.SecurityCodes?.MatchSecurityCode(message.PasswordResetCode);

            var securedPassword = await _passwordService.GenerateAsync(message.Password);

            message.User.Password = securedPassword.HashedPassword;
            message.User.PasswordProfileId = securedPassword.ProfileId;
            message.User.Salt = securedPassword.Salt;
            message.User.IsActive = true;
            message.User.ExpireSecurityCodesOfType(SecurityCodeType.AccessCode);
            message.User.ExpireSecurityCodesOfType(SecurityCodeType.PasswordResetCode);

            await _auditService.WriteAudit(new PasswordResetAuditMessage(message.User));

            await _userRepository.Update(message.User);
            _logger.Info($"Password changed for user id '{message.User.Id}'");

            return new PasswordResetResponse { ResetCode = resetCode };
        }
    }

    public class PasswordResetResponse
    {
        public SecurityCode ResetCode { get; set; }
    }
}
