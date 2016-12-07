using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : IValidator<ChangePasswordCommand>
    {
        private readonly IPasswordService _passwordService;

        public ChangePasswordCommandValidator(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public Task<ValidationResult> ValidateAsync(ChangePasswordCommand item)
        {
            var result = new ValidationResult();

            ValidateCurrentPasswordMatchesUser(item, result);
            ValidateNewPasswordMeetsSecurityRequirements(item, result);
            ValidateConfirmPasswordMatchesNewPassword(item, result);

            return Task.FromResult(result);
        }

        private void ValidateCurrentPasswordMatchesUser(ChangePasswordCommand command, ValidationResult result)
        {
            if (!_passwordService.VerifyAsync(command.CurrentPassword, command.User.Password,
                command.User.Salt, command.User.PasswordProfileId).Result)
            {
                result.AddError("CurrentPassword", "Invalid password");
            }
        }
        private void ValidateNewPasswordMeetsSecurityRequirements(ChangePasswordCommand command, ValidationResult result)
        {
            if (command.NewPassword.Length < 8 || command.NewPassword.Length > 16
                || !command.NewPassword.HasLowerCharacters() || !command.NewPassword.HasUpperCharacters()
                || !command.NewPassword.HasNumericCharacters())
            {
                result.AddError("NewPassword", "Password does not meet requirements");
            }
        }
        private void ValidateConfirmPasswordMatchesNewPassword(ChangePasswordCommand command, ValidationResult result)
        {
            if (command.NewPassword != command.ConfirmPassword)
            {
                result.AddError("ConfirmPassword", "Passwords do not match");
            }
        }
    }
}