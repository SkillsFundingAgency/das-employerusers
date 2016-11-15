using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ChangeEmail
{
    public class ChangeEmailCommandHandler : IAsyncRequestHandler<ChangeEmailCommand, Unit>
    {
        private readonly IValidator<ChangeEmailCommand> _validator;
        private readonly IUserRepository _userRepository;

        public ChangeEmailCommandHandler(IValidator<ChangeEmailCommand> validator, IUserRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(ChangeEmailCommand message)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            message.User.Email = message.User.PendingEmail;
            message.User.PendingEmail = null;
            message.User.ExpireSecurityCodesOfType(Domain.SecurityCodeType.ConfirmEmailCode);
            await _userRepository.Update(message.User);

            return Unit.Value;
        }
    }
}
