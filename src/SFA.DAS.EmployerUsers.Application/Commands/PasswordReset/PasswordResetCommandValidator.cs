using System;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class PasswordResetCommandValidator : IValidator<PasswordResetCommand>
    {
        public ValidationResult Validate(PasswordResetCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.User == null || !item.User.PasswordResetCode.Equals(item.PasswordResetCode, StringComparison.InvariantCultureIgnoreCase))
            {
                validationResult.AddError(nameof(item.PasswordResetCode), "Reset code is invalid, try again");
            }

            if (item.User == null || string.IsNullOrEmpty(item.Password) || string.IsNullOrEmpty(item.ConfirmPassword) || !item.Password.Equals(item.ConfirmPassword))
            {
                validationResult.AddError(nameof(item.ConfirmPassword), "Sorry, your passwords don’t match");
            }

            return validationResult;
        }
    }
}
