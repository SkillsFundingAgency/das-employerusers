using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : IValidator<ChangePasswordCommand>
    {
        private readonly IPasswordService _passwordService;
        private readonly IConfigurationService _configurationService;

        public ChangePasswordCommandValidator(IPasswordService passwordService, IConfigurationService configurationService)
        {
            _passwordService = passwordService;
            _configurationService = configurationService;
        }

        public async Task<ValidationResult> ValidateAsync(ChangePasswordCommand item)
        {
            var result = new ValidationResult();

            if (!ValidateComandHasRequiredValues(item, result))
            {
                return result;
            }

            await ValidateCurrentPasswordMatchesUser(item, result);
            await ValidateNewPasswordNotInRecentHistory(item, result);
            ValidateNewPasswordMeetsSecurityRequirements(item, result);
            ValidateConfirmPasswordMatchesNewPassword(item, result);

            return result;
        }

        private bool ValidateComandHasRequiredValues(ChangePasswordCommand command, ValidationResult result)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(command.CurrentPassword))
            {
                isValid = false;
                result.AddError(nameof(command.CurrentPassword), "Current password is required");
            }
            if (string.IsNullOrEmpty(command.NewPassword))
            {
                isValid = false;
                result.AddError(nameof(command.NewPassword), "New password is required");
            }

            return isValid;
        }

        private async Task ValidateCurrentPasswordMatchesUser(ChangePasswordCommand command, ValidationResult result)
        {
            var passwordsMatch = await _passwordService.VerifyAsync(command.CurrentPassword, command.User.Password,
                command.User.Salt, command.User.PasswordProfileId);
            if (!passwordsMatch)
            {
                result.AddError(nameof(command.CurrentPassword), "Invalid password");
            }
        }

        private async Task ValidateNewPasswordNotInRecentHistory(ChangePasswordCommand command, ValidationResult result)
        {
            var config = await _configurationService.GetAsync<EmployerUsersConfiguration>();
            var recentPasswords = command.User.PasswordHistory.OrderByDescending(p => p.DateSet).Take(config.Account.NumberOfPasswordsInHistory);

            foreach (var historicPassword in recentPasswords)
            {
                if (await _passwordService.VerifyAsync(command.NewPassword, historicPassword.Password, historicPassword.Salt, historicPassword.PasswordProfileId))
                {
                    result.AddError(nameof(command.NewPassword), $"Password has been used too recently. You cannot use your last {config.Account.NumberOfPasswordsInHistory} passwords");
                    return;
                }
            }
        }

        private void ValidateNewPasswordMeetsSecurityRequirements(ChangePasswordCommand command, ValidationResult result)
        {
            if (!_passwordService.CheckPasswordMatchesRequiredComplexity(command.NewPassword))
            {
                result.AddError(nameof(command.NewPassword), "Your password must contain upper and lowercase letters, a number and at least 8 characters");
            }
        }

        private void ValidateConfirmPasswordMatchesNewPassword(ChangePasswordCommand command, ValidationResult result)
        {
            if (command.NewPassword != command.ConfirmPassword)
            {
                result.AddError(nameof(command.ConfirmPassword), "Passwords do not match");
            }
        }
    }
}