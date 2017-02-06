using System.Threading.Tasks;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : BaseValidator, IValidator<RegisterUserCommand>
    {
        private readonly IPasswordService _passwordService;

        public RegisterUserCommandValidator(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public Task<ValidationResult> ValidateAsync(RegisterUserCommand item)
        {
            var validationResult = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(item.Email) || !IsEmailValid(item.Email))
            {
                validationResult.AddError(nameof(item.Email), "Enter a valid email address");
            }

            if (string.IsNullOrEmpty(item.FirstName))
            {
                validationResult.AddError(nameof(item.FirstName), "Enter first name");
            }

            if (string.IsNullOrEmpty(item.LastName))
            {
                validationResult.AddError(nameof(item.LastName), "Enter last name");
            }

            if (string.IsNullOrEmpty(item.Password))
            {
                validationResult.AddError(nameof(item.Password), "Enter password");
            }
            else if (!_passwordService.CheckPasswordMatchesRequiredComplexity(item.Password))
            {
                validationResult.AddError(nameof(item.Password), "Password requires upper and lowercase letters, a number and at least 8 characters");
            }

            if (string.IsNullOrEmpty(item.ConfirmPassword))
            {
                validationResult.AddError(nameof(item.ConfirmPassword), "Re-type password");
            }
            else if (!string.IsNullOrEmpty(item.Password) && !item.ConfirmPassword.Equals(item.Password))
            {
                validationResult.AddError(nameof(item.ConfirmPassword), "Passwords don't match");
            }

            if (!item.HasAcceptedTermsAndConditions)
            {
                validationResult.AddError(nameof(item.HasAcceptedTermsAndConditions), "You need to accept the terms of use");
            }

            return Task.FromResult(validationResult);
        }
        
    }
}