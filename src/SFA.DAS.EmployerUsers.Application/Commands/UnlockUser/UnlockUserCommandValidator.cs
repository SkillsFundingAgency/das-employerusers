using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommandValidator : IValidator<UnlockUserCommand>
    {
        public Task<ValidationResult> ValidateAsync(UnlockUserCommand item)
        {
            var result = new ValidationResult();
            if (string.IsNullOrEmpty(item.Email))
            {
                result.ValidationDictionary.Add("Email", "Enter an email address");
            }
            if (string.IsNullOrEmpty(item.UnlockCode))
            {
                result.ValidationDictionary.Add("UnlockCode", "Enter an unlock code");
            }

            if (item.User == null)
            {
                result.ValidationDictionary.Add("User", "That account does not exist");
                return Task.FromResult(result);
            }

            var matchingUnlockCode = item.User.SecurityCodes?.OrderByDescending(sc => sc.ExpiryTime)
                                                             .FirstOrDefault(sc => sc.Code.Equals(item.UnlockCode, StringComparison.CurrentCultureIgnoreCase)
                                                                                && sc.CodeType == Domain.SecurityCodeType.UnlockCode);

            if (matchingUnlockCode == null)
            {
                result.ValidationDictionary.Add("UnlockCodeMatch", "Unlock code is not correct");
            }
            else if (matchingUnlockCode.ExpiryTime < DateTime.UtcNow)
            {
                result.ValidationDictionary.Add("UnlockCodeExpiry", "Unlock code has expired");
                return Task.FromResult(result);
            }

            return Task.FromResult(result);
        }
    }
}