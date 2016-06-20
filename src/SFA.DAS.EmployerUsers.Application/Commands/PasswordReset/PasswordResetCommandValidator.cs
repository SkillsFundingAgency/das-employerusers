using System;
using System.Text.RegularExpressions;
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

        public ValidationResult Validate(PasswordResetCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.User == null || !item.User.PasswordResetCode.Equals(item.PasswordResetCode, StringComparison.InvariantCultureIgnoreCase))
            {
                validationResult.AddError(nameof(item.PasswordResetCode), "Reset code is invalid, try again");
            }

            if (string.IsNullOrEmpty(item.Password))
            {
                validationResult.AddError(nameof(item.Password), "Please enter password");
            }
            else if (!_passwordService.CheckPasswordMatchesRequiredComplexity(item.Password))
            {
                validationResult.AddError(nameof(item.Password), "Password requires upper and lowercase letters, a number and at least 8 characters");
            }

            if (string.IsNullOrEmpty(item.ConfirmPassword))
            {
                validationResult.AddError(nameof(item.ConfirmPassword), "Please confirm password");
            }
            else if (!string.IsNullOrEmpty(item.Password) && !item.ConfirmPassword.Equals(item.Password))
            {
                validationResult.AddError(nameof(item.ConfirmPassword), "Sorry, your passwords don’t match");
            }


            return validationResult;
        }
        
    }
}
