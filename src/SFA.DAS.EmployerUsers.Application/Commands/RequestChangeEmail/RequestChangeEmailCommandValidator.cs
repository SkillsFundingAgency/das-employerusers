using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.RequestChangeEmail
{
    public class RequestChangeEmailCommandValidator : BaseValidator, IValidator<RequestChangeEmailCommand>
    {
        public Task<ValidationResult> ValidateAsync(RequestChangeEmailCommand item)
        {
            var result = new ValidationResult();

            if (string.IsNullOrEmpty(item.UserId))
            {
                result.AddError(nameof(item.UserId));
            }

            if (string.IsNullOrEmpty(item.NewEmailAddress) || !IsEmailValid(item.NewEmailAddress))
            {
                result.AddError(nameof(item.NewEmailAddress), "Enter a valid email address");
            }

            if (string.IsNullOrEmpty(item.ConfirmEmailAddress) || !IsEmailValid(item.ConfirmEmailAddress))
            {
                result.AddError(nameof(item.ConfirmEmailAddress), "Re-type email address");
            }

            if (!result.IsValid())
            {
                return Task.FromResult(result);
            }

            if (!item.NewEmailAddress.Equals(item.ConfirmEmailAddress, System.StringComparison.CurrentCultureIgnoreCase))
            {
                result.AddError(nameof(item.ConfirmEmailAddress), "Emails don't match");
            }

            return Task.FromResult(result);
        }
    }
}