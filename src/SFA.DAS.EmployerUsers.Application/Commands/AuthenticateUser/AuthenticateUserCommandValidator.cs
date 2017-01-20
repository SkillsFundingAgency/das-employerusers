using System;
using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandValidator : BaseValidator, IValidator<AuthenticateUserCommand>
    {
        public Task<ValidationResult> ValidateAsync(AuthenticateUserCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.EmailAddress) || !IsEmailValid(item.EmailAddress))
            {
                validationResult.AddError(nameof(item.EmailAddress), "Enter a valid email address");
            }

            if (string.IsNullOrEmpty(item.Password))
            {
                validationResult.AddError(nameof(item.Password), "Enter password");
            }

            return Task.FromResult(validationResult);
        }
    }
}
