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
            var validationResult = new ValidationResult
            {
                ValidationDictionary = new Dictionary<string, string>()
            };

            if (TheUserIsBeingClassedAsValidFromJustHavingAMatchingEmail(item))
            {
                return Task.FromResult(validationResult);
            }

            if (string.IsNullOrWhiteSpace(item?.AccessCode))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { nameof(item.AccessCode), "Missing code" } };
                return Task.FromResult(validationResult);
            }

            if (string.IsNullOrEmpty(item.UserId))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { nameof(item.AccessCode), "Invalid code" } };
                return Task.FromResult(validationResult);
            }

            var matchingAccessCodes =
                item.User.SecurityCodes.Where(sc => sc.CodeType == Domain.SecurityCodeType.AccessCode
                                                  && sc.Code.Equals(item.AccessCode,
                                                      StringComparison.CurrentCultureIgnoreCase)).ToList();

            if (!matchingAccessCodes.Any())
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { nameof(item.AccessCode), "Invalid code" } };
                return Task.FromResult(validationResult);
            }

            if(!matchingAccessCodes.All(x => x.ExpiryTime >= DateTime.UtcNow))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { nameof(item.AccessCode) + "Expired", "Your code has expired" } };
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
