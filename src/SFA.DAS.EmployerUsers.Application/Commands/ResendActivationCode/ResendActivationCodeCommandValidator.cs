using System.Collections.Generic;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode
{
    public class ResendActivationCodeCommandValidator : IValidator<ResendActivationCodeCommand>
    {
        public ValidationResult Validate(ResendActivationCodeCommand item)
        {
            var validate = !string.IsNullOrEmpty(item?.UserId);

            return validate ? new ValidationResult { ValidationDictionary = new Dictionary<string, string>() }
                            : new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } };
        }
    }
}