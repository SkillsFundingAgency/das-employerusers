using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.Commands.RequestChangeEmail
{
    public class RequestChangeEmailCommandValidator : BaseValidator, IValidator<RequestChangeEmailCommand>
    {
        private readonly IUserRepository _userRepository;

        public RequestChangeEmailCommandValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ValidationResult> ValidateAsync(RequestChangeEmailCommand item)
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
                return result;
            }

            if (!item.NewEmailAddress.Equals(item.ConfirmEmailAddress, System.StringComparison.CurrentCultureIgnoreCase))
            {
                result.AddError(nameof(item.ConfirmEmailAddress), "Emails don't match");
            }

            if (result.IsValid())
            {
                var user = await _userRepository.GetByEmailAddress(item.NewEmailAddress);
                if (user != null)
                {
                    result.AddError(nameof(item.NewEmailAddress), "Email address already registered");
                }
            }

            return result;
        }
    }
}