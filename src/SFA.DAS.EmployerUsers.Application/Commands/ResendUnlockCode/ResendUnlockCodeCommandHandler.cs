using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode
{
    public class ResendUnlockCodeCommandHandler : AsyncRequestHandler<ResendUnlockCodeCommand>
    {
        private readonly IValidator<ResendUnlockCodeCommand> _validator;
        private readonly IMediator _mediator;
        private readonly IAuditService _auditService;

        public ResendUnlockCodeCommandHandler(
            IValidator<ResendUnlockCodeCommand> validator, 
            IMediator mediator, 
            IAuditService auditService)
        {
            _validator = validator;
            _mediator = mediator;
            _auditService = auditService;
        }

        protected override async Task HandleCore(ResendUnlockCodeCommand message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            await _mediator.PublishAsync(new AccountLockedEvent
            {
                User = new User
                {
                    Email = message.Email
                },
                ResendUnlockCode = true,
                ReturnUrl = message.ReturnUrl
            });
        }
    }
}