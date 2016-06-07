using System.Collections.Generic;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode
{
    public class ResendActivationCodeCommandValidator : IValidator<ResendActivationCodeCommand>
    {
        public ValidationResult Validate(ResendActivationCodeCommand item)
        {
            var validationErrors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(item?.UserId))
            {
                validationErrors.Add("UserId", "UserId has not been specified");
            }

            return new ValidationResult {ValidationDictionary = validationErrors};
        }
    }
}