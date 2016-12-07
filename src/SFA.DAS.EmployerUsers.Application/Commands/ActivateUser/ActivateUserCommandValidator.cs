using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ActivateUser
{
    public class ActivateUserCommandValidator : IValidator<ActivateUserCommand>
    {
        public Task<ValidationResult> ValidateAsync(ActivateUserCommand item)
        {
            var validationResult = new ValidationResult();
            validationResult.ValidationDictionary = new Dictionary<string, string>();

            if (TheUserIsBeingClassedAsValidFromJustHavingAMatchingEmail(item))
            {
                return Task.FromResult(validationResult);
            }

            if (string.IsNullOrEmpty(item?.AccessCode) || string.IsNullOrEmpty(item.UserId))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { "", "" } };
                return Task.FromResult(validationResult);
            }
            
            if (!item.User.SecurityCodes.Any(sc => sc.CodeType == Domain.SecurityCodeType.AccessCode 
                                                && sc.Code.Equals(item.AccessCode, StringComparison.CurrentCultureIgnoreCase)
                                                && sc.ExpiryTime >= DateTime.UtcNow))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { "", "" } };
                return Task.FromResult(validationResult);
            }

            return Task.FromResult(validationResult);
        }

        private static bool TheUserIsBeingClassedAsValidFromJustHavingAMatchingEmail(ActivateUserCommand item)
        {
            return !string.IsNullOrEmpty(item?.Email) || (item?.User?.Email != null
                                                            && string.IsNullOrEmpty(item.UserId)
                                                            && !item.Email.Equals(item.User.Email, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
