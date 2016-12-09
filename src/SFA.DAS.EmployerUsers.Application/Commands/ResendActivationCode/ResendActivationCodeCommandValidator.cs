using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode
{
    public class ResendActivationCodeCommandValidator : IValidator<ResendActivationCodeCommand>
    {
        public Task<ValidationResult> ValidateAsync(ResendActivationCodeCommand item)
        {
            var validationErrors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(item?.UserId))
            {
                validationErrors.Add("UserId", "UserId has not been specified");
            }

            return Task.FromResult(new ValidationResult {ValidationDictionary = validationErrors});
        }
    }
}