using System;
using System.Configuration;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Extensions;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.PasswordReset
{
    public class ValidatePasswordResetCodeCommandValidator : IValidator<ValidatePasswordResetCodeCommand>
    {
        public ValidatePasswordResetCodeCommandValidator()
        {
        }

        public Task<ValidationResult> ValidateAsync(ValidatePasswordResetCodeCommand item)
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

            return Task.FromResult(validationResult);
        }
    }
}
