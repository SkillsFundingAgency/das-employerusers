using System;
using System.Configuration;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Extensions;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class PasswordResetCommandValidator : IValidator<PasswordResetCommand>
    {
        private readonly IPasswordService _passwordService;

        public PasswordResetCommandValidator(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public Task<ValidationResult> ValidateAsync(PasswordResetCommand item)
        {
            var validationResult = new ValidationResult();

            var resetCode = item.User?.SecurityCodes?.MatchSecurityCode(item.PasswordResetCode);

            if (resetCode == null)
            {
                validationResult.AddError(nameof(item.PasswordResetCode), "Reset code is invalid");
            }
            else if (resetCode.ExpiryTime < DateTime.UtcNow && ConfigurationManager.AppSettings["UseStaticCodeGenerator"].Equals("false", StringComparison.CurrentCultureIgnoreCase))
            {
                validationResult.AddError(nameof(item.PasswordResetCode), "Reset code has expired");
            }

            if (string.IsNullOrEmpty(item.Password))
            {
                validationResult.AddError(nameof(item.Password), "Enter a password");
            }
            else if (!_passwordService.CheckPasswordMatchesRequiredComplexity(item.Password))
            {
                validationResult.AddError(nameof(item.Password), "Password requires upper and lowercase letters, a number and at least 8 characters");
            }

            if (string.IsNullOrEmpty(item.ConfirmPassword))
            {
                validationResult.AddError(nameof(item.ConfirmPassword), "Re-type password");
            }
            else if (!string.IsNullOrEmpty(item.Password) && !item.ConfirmPassword.Equals(item.Password))
            {
                validationResult.AddError(nameof(item.ConfirmPassword), "Passwords don’t match");
            }


            return Task.FromResult(validationResult);
        }

    }
}
