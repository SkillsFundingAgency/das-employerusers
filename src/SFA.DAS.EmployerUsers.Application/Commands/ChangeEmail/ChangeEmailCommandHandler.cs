using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ChangeEmail
{
    public class ChangeEmailCommandHandler : IAsyncRequestHandler<ChangeEmailCommand, ChangeEmailCommandResult>
    {
        private readonly IValidator<ChangeEmailCommand> _validator;
        private readonly IUserRepository _userRepository;

        public ChangeEmailCommandHandler(IValidator<ChangeEmailCommand> validator, IUserRepository userRepository)
        {
            _validator = validator;
            _userRepository = userRepository;
        }

        public async Task<ChangeEmailCommandResult> Handle(ChangeEmailCommand message)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var securityCode = message.User.SecurityCodes.Single(sc => sc.Code == message.SecurityCode
                                                                    && sc.CodeType == Domain.SecurityCodeType.ConfirmEmailCode);

            message.User.Email = message.User.PendingEmail;
            message.User.PendingEmail = null;
            message.User.ExpireSecurityCodesOfType(Domain.SecurityCodeType.ConfirmEmailCode);
            await _userRepository.Update(message.User);

            return new ChangeEmailCommandResult
            {
                ReturnUrl = securityCode.ReturnUrl
            };
        }
    }
}
