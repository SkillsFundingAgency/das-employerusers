using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.UnlockUser
{
    public class UnlockUserCommandValidator : IValidator<UnlockUserCommand>
    {
        public ValidationResult Validate(UnlockUserCommand item)
        {
            var result = new ValidationResult();
            if (string.IsNullOrEmpty(item.Email))
            {
                result.ValidationDictionary.Add("Email","Email has not been supplied");
            }
            if (string.IsNullOrEmpty(item.UnlockCode))
            {
                result.ValidationDictionary.Add("UnlockCode","Unlock Code has not been supplied");
            }

            return result;
        }
    }
}