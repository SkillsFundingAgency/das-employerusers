using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommandValidator : BaseValidator, IValidator<UnlockUserCommand>
    {
        public Task<ValidationResult> ValidateAsync(UnlockUserCommand item)
        {
            var result = new ValidationResult();
            if (string.IsNullOrEmpty(item.Email))
            {
                result.AddError(nameof(item.Email), "Enter an email address");
            }
            if (!string.IsNullOrEmpty(item.Email) && !IsEmailValid(item.Email))
            {
                result.AddError(nameof(item.Email), "Enter a valid email address");
            }
            if (string.IsNullOrEmpty(item.UnlockCode))
            {
                result.AddError(nameof(item.UnlockCode), "Enter an unlock code");
            }

            if (!string.IsNullOrEmpty(item.Email) && !string.IsNullOrEmpty(item.UnlockCode) && item.User == null)
            {
                result.AddError(nameof(item.UnlockCode), "Unlock code is not correct");
            }

            if (!result.IsValid())
            {
                return Task.FromResult(result);
            }


            var matchingUnlockCode = item.User.SecurityCodes?.OrderByDescending(sc => sc.ExpiryTime)
                                                             .FirstOrDefault(sc => sc.Code.Equals(item.UnlockCode, StringComparison.CurrentCultureIgnoreCase)
                                                                                && sc.CodeType == Domain.SecurityCodeType.UnlockCode);

            if (matchingUnlockCode == null)
            {
                result.AddError(nameof(item.UnlockCode), "Unlock code is not correct");
            }
            else if (matchingUnlockCode.ExpiryTime < DateTime.UtcNow && ConfigurationManager.AppSettings["UseStaticCodeGenerator"].Equals("false", StringComparison.CurrentCultureIgnoreCase))
            {
                result.AddError(nameof(item.UnlockCode), "Unlock code has expired");
                return Task.FromResult(result);
            }

            return Task.FromResult(result);
        }
    }
}