using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.ChangeEmail;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ChangeEmail
{
    public class ChangeEmailCommandHandler : IAsyncRequestHandler<ChangeEmailCommand, ChangeEmailCommandResult>
    {
        private readonly IValidator<ChangeEmailCommand> _validator;
        private readonly IUserRepository _userRepository;
        private readonly IAuditService _auditService;

        public ChangeEmailCommandHandler(IValidator<ChangeEmailCommand> validator, IUserRepository userRepository, IAuditService auditService)
        {
            _validator = validator;
            _userRepository = userRepository;
            _auditService = auditService;
        }

        public async Task<ChangeEmailCommandResult> Handle(ChangeEmailCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var securityCode = message.User.SecurityCodes.Single(sc => sc.Code.Equals(message.SecurityCode, System.StringComparison.CurrentCultureIgnoreCase)
                                                                    && sc.CodeType == Domain.SecurityCodeType.ConfirmEmailCode);
            var oldEmail = message.User.Email;

            message.User.Email = securityCode.PendingValue;
            message.User.ExpireSecurityCodesOfType(Domain.SecurityCodeType.ConfirmEmailCode);
            await _userRepository.Update(message.User);

            await _auditService.WriteAudit(new CompleteChangeEmailAuditMessage(message.User, oldEmail));

            return new ChangeEmailCommandResult
            {
                ReturnUrl = securityCode.ReturnUrl
            };
        }
    }
}
