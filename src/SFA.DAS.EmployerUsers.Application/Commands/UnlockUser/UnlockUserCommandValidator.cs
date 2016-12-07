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
                result.ValidationDictionary.Add("Email", "Email has not been supplied");
            }
            if (string.IsNullOrEmpty(item.UnlockCode))
            {
                result.ValidationDictionary.Add("UnlockCode", "Unlock Code has not been supplied");
            }

            if (item.User == null)
            {
                result.ValidationDictionary.Add("User", "User Does Not Exist");
                return Task.FromResult(result);
            }

            var matchingUnlockCode = item.User.SecurityCodes?.OrderByDescending(sc => sc.ExpiryTime)
                                                             .FirstOrDefault(sc => sc.Code.Equals(item.UnlockCode, StringComparison.CurrentCultureIgnoreCase)
                                                                                && sc.CodeType == Domain.SecurityCodeType.UnlockCode);

            if (matchingUnlockCode == null)
            {
                result.ValidationDictionary.Add("UnlockCodeMatch", "Unlock Code is not correct");
            }
            else if (matchingUnlockCode.ExpiryTime < DateTime.UtcNow)
            {
                result.ValidationDictionary.Add("UnlockCodeExpiry", "Unlock Code has expired");
                return Task.FromResult(result);
            }

            return Task.FromResult(result);
        }
    }
}