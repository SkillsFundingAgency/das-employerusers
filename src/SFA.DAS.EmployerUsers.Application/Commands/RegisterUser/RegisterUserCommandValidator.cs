using System.Collections.Generic;
using System.Text.RegularExpressions;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : IValidator<RegisterUserCommand>
    {
        public ValidationResult Validate(RegisterUserCommand item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(item?.Email) || string.IsNullOrWhiteSpace(item.FirstName) ||
                string.IsNullOrWhiteSpace(item.LastName) || string.IsNullOrWhiteSpace(item.Password) ||
                string.IsNullOrWhiteSpace(item.ConfirmPassword))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { "", "" } };
                return validationResult;
            }

            if (CheckPasswordMatchesAtLeastOneUppercaseOneLowercaseOneNumberAndAtLeastEightCharacters(item.Password))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { "", "" } };
                return validationResult;
            }

            if (!item.ConfirmPassword.Equals(item.Password))
            {
                validationResult.ValidationDictionary = new Dictionary<string, string> { { "", "" } };
                return validationResult;
            }

            return validationResult;
        }

        private static bool CheckPasswordMatchesAtLeastOneUppercaseOneLowercaseOneNumberAndAtLeastEightCharacters(string password)
        {
            return !Regex.IsMatch(password, @"^(?=(.*[0-9].*))(?=(.*[a-z].*))(?=(.*[A-Z].*)).{8,}$");
        }
    }
}