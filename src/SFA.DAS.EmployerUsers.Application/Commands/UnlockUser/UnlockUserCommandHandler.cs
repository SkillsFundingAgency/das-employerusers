using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Unlock;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommandHandler : IAsyncRequestHandler<UnlockUserCommand, UnlockUserResponse>
    {
        private readonly IValidator<UnlockUserCommand> _unlockUserCommandValidator;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly IAuditService _auditService;

        public UnlockUserCommandHandler(IValidator<UnlockUserCommand> unlockUserCommandValidator, IUserRepository userRepository, IMediator mediator, IAuditService auditService)
        {
            _unlockUserCommandValidator = unlockUserCommandValidator;
            _userRepository = userRepository;
            _mediator = mediator;
            _auditService = auditService;
        }

        public async Task<UnlockUserResponse> Handle(UnlockUserCommand message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(typeof(UnlockUserCommand).Name, "Unlock User Command Is Null");
            }

            message.User = await _userRepository.GetByEmailAddress(message.Email);

            if (message.User != null && !message.User.IsLocked)
            {
                return null;
            }

            var result = await _unlockUserCommandValidator.ValidateAsync(message);

            if (!result.IsValid())
            {
                if (message.User != null)
                {
                    await _mediator.PublishAsync(new AccountLockedEvent { User = message.User });
                    await _auditService.WriteAudit(new FailedUnlockAuditMessage(message.User, message.UnlockCode));
                }
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            message.User.FailedLoginAttempts = 0;
            message.User.IsLocked = false;
            var matchingUnlockCode = message.User.SecurityCodes?.OrderByDescending(sc => sc.ExpiryTime)
                                                           .FirstOrDefault(sc => sc.Code.Equals(message.UnlockCode, StringComparison.CurrentCultureIgnoreCase)
                                                                              && sc.CodeType == Domain.SecurityCodeType.UnlockCode);
            message.User.ExpireSecurityCodesOfType(Domain.SecurityCodeType.UnlockCode);


            await _userRepository.Update(message.User);

            await _auditService.WriteAudit(new UnlockedAuditMessage(message.User));

            return new UnlockUserResponse() { UnlockCode = matchingUnlockCode };
        }


    }

    public class UnlockUserResponse
    {
        public SecurityCode UnlockCode { get; set; }
    }
}
