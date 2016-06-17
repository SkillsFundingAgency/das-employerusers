using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode
{
    public class RequestPasswordResetCodeCommandValidator : IValidator<RequestPasswordResetCodeCommand>
    {
        public ValidationResult Validate(RequestPasswordResetCodeCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item.Email))
            {
                validationResult.AddError(nameof(item.Email), "Please enter email address");
            }

            return validationResult;
        }
    }
}