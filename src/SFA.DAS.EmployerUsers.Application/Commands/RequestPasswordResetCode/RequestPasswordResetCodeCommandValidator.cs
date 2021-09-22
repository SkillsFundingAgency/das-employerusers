using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;
using ValidationResult = SFA.DAS.EmployerUsers.Application.Validation.ValidationResult;

namespace SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode
{
    public class RequestPasswordResetCodeCommandValidator : BaseValidator, IValidator<RequestPasswordResetCodeCommand>
    {
        public Task<ValidationResult> ValidateAsync(RequestPasswordResetCodeCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.Email) || !base.IsEmailValid(item.Email))
            {
                validationResult.AddError(nameof(item.Email), "Enter a valid email address");
            }

            return Task.FromResult(validationResult);
        }
    }
}