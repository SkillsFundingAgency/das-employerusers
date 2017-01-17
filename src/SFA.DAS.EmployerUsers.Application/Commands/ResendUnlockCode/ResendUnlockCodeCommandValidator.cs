using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode
{
    public class ResendUnlockCodeCommandValidator :BaseValidator, IValidator<ResendUnlockCodeCommand>
    {
        public Task<ValidationResult> ValidateAsync(ResendUnlockCodeCommand item)
        {
            var result = new ValidationResult();
            if (string.IsNullOrEmpty(item.Email))
            {
                result.AddError(nameof(item.Email), "Please enter email address");
            }

            if (!string.IsNullOrEmpty(item.Email) && !IsEmailValid(item.Email))
            {
                result.AddError(nameof(item.Email), "Please enter a valid email address");
            }

            return Task.FromResult(result);
        }
    }
}
