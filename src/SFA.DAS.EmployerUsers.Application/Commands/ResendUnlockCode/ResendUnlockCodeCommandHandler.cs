using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode
{
    public class ResendUnlockCodeCommandHandler : AsyncRequestHandler<ResendUnlockCodeCommand>
    {
        private readonly IValidator<ResendUnlockCodeCommand> _validator;
        private readonly IMediator _mediator;

        public ResendUnlockCodeCommandHandler(IValidator<ResendUnlockCodeCommand> validator, IMediator mediator)
        {
            _validator = validator;
            _mediator = mediator;
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
                ResendUnlockCode = true
            });
        }
    }
}