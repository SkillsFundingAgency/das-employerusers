using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IAsyncRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly IValidator<ChangePasswordCommand> _validator;
        private readonly IPasswordService _passwordService;
        private readonly IUserRepository _userRepository;

        public ChangePasswordCommandHandler(IValidator<ChangePasswordCommand> validator, IPasswordService passwordService, IUserRepository userRepository)
        {
            _validator = validator;
            _passwordService = passwordService;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(ChangePasswordCommand message)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await UpdateUserWithNewPassword(message.User, message.NewPassword);
            await _userRepository.Update(message.User);

            return Unit.Value;
        }

        private async Task UpdateUserWithNewPassword(User user, string newPassword)
        {
            var securePassword = await _passwordService.GenerateAsync(newPassword);

            user.Password = securePassword.HashedPassword;
            user.Salt = securePassword.Salt;
            user.PasswordProfileId = securePassword.ProfileId;
        }
    }
}