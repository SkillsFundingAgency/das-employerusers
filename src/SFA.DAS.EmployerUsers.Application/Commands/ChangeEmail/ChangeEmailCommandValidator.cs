using System;
using System.Linq;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.ChangeEmail
{
    public class ChangeEmailCommandValidator : IValidator<ChangeEmailCommand>
    {
        private readonly IPasswordService _passwordService;

        public ChangeEmailCommandValidator(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public ValidationResult Validate(ChangeEmailCommand item)
        {
            var result = new ValidationResult();

            if (!ValidateRequiredPropertiesSetOnCommand(item, result))
            {
                return result;
            }

            ValidatePassword(item, result);
            ValidateSecurityCode(item, result);

            return result;
        }


        private bool ValidateRequiredPropertiesSetOnCommand(ChangeEmailCommand command, ValidationResult result)
        {
            var hasAllPropertiesSet = true;

            if (command.User == null)
            {
                result.AddError("User", "User Does Not Exist");
                hasAllPropertiesSet = false;
            }

            if (string.IsNullOrEmpty(command.SecurityCode))
            {
                result.AddError("SecurityCode", "Security code has not been provided");
                hasAllPropertiesSet = false;
            }

            if (string.IsNullOrEmpty(command.Password))
            {
                result.AddError("Password", "Password has not been provided");
                hasAllPropertiesSet = false;
            }

            return hasAllPropertiesSet;
        }
        private void ValidatePassword(ChangeEmailCommand command, ValidationResult result)
        {
            if (!_passwordService.VerifyAsync(command.Password, command.User.Password, command.User.Salt, command.User.PasswordProfileId).Result)
            {
                result.AddError("Password", "Password is incorrect");
            }
        }
        private void ValidateSecurityCode(ChangeEmailCommand command, ValidationResult result)
        {
            var securityCode = command.User.SecurityCodes.FirstOrDefault(c => c.Code.Equals(command.SecurityCode, System.StringComparison.CurrentCultureIgnoreCase)
                                                                          && c.CodeType == Domain.SecurityCodeType.ConfirmEmailCode);
            if (securityCode == null)
            {
                result.AddError("SecurityCode", "Security code is incorrect");
            }
            else if (securityCode.ExpiryTime < DateTime.UtcNow)
            {
                result.AddError("SecurityCode", "Security code has expired");
            }
        }
    }
}
