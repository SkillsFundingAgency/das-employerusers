using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode
{
    public class ResendUnlockCodeCommandValidator : IValidator<ResendUnlockCodeCommand>
    {
        public ValidationResult Validate(ResendUnlockCodeCommand item)
        {
            var result = new ValidationResult();
            if (string.IsNullOrEmpty(item.Email))
            {
                result.ValidationDictionary.Add(nameof(item.Email), "Please enter email address");
            }

            return result;
        }
    }
}
