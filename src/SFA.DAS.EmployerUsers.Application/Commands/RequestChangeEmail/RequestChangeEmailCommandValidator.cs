using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.RequestChangeEmail
{
    public class RequestChangeEmailCommandValidator : IValidator<RequestChangeEmailCommand>
    {
        public Task<ValidationResult> ValidateAsync(RequestChangeEmailCommand item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.UserId) || string.IsNullOrEmpty(item.NewEmailAddress))
            {
                result.ValidationDictionary.Add("", "");
                return Task.FromResult(result);
            }

            if (!item.NewEmailAddress.Equals(item.ConfirmEmailAddress, System.StringComparison.CurrentCultureIgnoreCase))
            {
                result.ValidationDictionary.Add("ConfirmEmailAddress", "Confirm email address does not match new email address");
            }

            return Task.FromResult(result);
        }
    }
}