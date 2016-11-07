using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ActivateUser
{
    public class ActivateUserCommandValidator : IValidator<ActivateUserCommand>
    {
        public ValidationResult Validate(ActivateUserCommand item)
        {
            var validationResult = new ValidationResult();
            validationResult.ValidationDictionary = new Dictionary<string, string>();

            if (TheUserIsBeingClassedAsValidFromJustHavingAMatchingEmail(item))
            {
                return validationResult;
            }

            if (string.IsNullOrEmpty(item?.AccessCode) || string.IsNullOrEmpty(item.UserId))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { "", "" } };
                return validationResult;
            }
            
            if (!item.User.SecurityCodes.Any(sc => sc.CodeType == Domain.SecurityCodeType.AccessCode 
                                                && sc.Code.Equals(item.AccessCode, StringComparison.CurrentCultureIgnoreCase)
                                                && sc.ExpiryTime >= DateTime.Now))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { "", "" } };
                return validationResult;
            }

            return validationResult;
        }

        private static bool TheUserIsBeingClassedAsValidFromJustHavingAMatchingEmail(ActivateUserCommand item)
        {
            return !string.IsNullOrEmpty(item?.Email) || (item?.User?.Email != null
                                                            && string.IsNullOrEmpty(item.UserId)
                                                            && !item.Email.Equals(item.User.Email, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
