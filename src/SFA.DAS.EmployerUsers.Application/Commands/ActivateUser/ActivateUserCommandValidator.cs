using System;
using System.Collections.Generic;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ActivateUser
{
    public class ActivateUserCommandValidator : IValidator<ActivateUserCommand>
    {
        public ValidationResult Validate(ActivateUserCommand item)
        {
            var validationResult = new ValidationResult();
            if (string.IsNullOrEmpty(item?.AccessCode) || string.IsNullOrEmpty(item.UserId))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> {{"", ""}};
                return validationResult;
            }

            if (!item.AccessCode.Equals(item.User.AccessCode, StringComparison.CurrentCultureIgnoreCase))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { "", "" } };
                return validationResult;
            }

            return validationResult;
        }
    }
}
