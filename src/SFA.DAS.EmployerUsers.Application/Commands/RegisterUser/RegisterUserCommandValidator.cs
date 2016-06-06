using System;
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
            
            if (string.IsNullOrWhiteSpace(item.Email))
            {
                validationResult.AddError("Email", "Please enter email address");
            }

            if (string.IsNullOrEmpty(item.FirstName))
            {
                validationResult.AddError("FirstName", "Please enter first name");
            }

            if (string.IsNullOrEmpty(item.LastName))
            {
                validationResult.AddError("LastName", "Please enter last name");
            }

            if (string.IsNullOrEmpty(item.Password))
            {
                validationResult.AddError("Password", "Please enter password");
            }

            if (string.IsNullOrEmpty(item.ConfirmPassword))
            {
                validationResult.AddError("ConfirmPassword", "Please confirm password");
            }

            if (!string.IsNullOrEmpty(item.Password) && CheckPasswordMatchesAtLeastOneUppercaseOneLowercaseOneNumberAndAtLeastEightCharacters(item.Password))
            {
                validationResult.AddError("PasswordComplexity", "Password requires upper and lowercase letters, a number and at least 8 characters");
            }

            if (!string.IsNullOrEmpty(item.Password) && !string.IsNullOrEmpty(item.ConfirmPassword) && !item.ConfirmPassword.Equals(item.Password))
            {
                validationResult.AddError("PasswordNotMatch", "Sorry, your passwords don’t match");
            }

            return validationResult;
        }

        private static bool CheckPasswordMatchesAtLeastOneUppercaseOneLowercaseOneNumberAndAtLeastEightCharacters(string password)
        {
            return !Regex.IsMatch(password, @"^(?=(.*[0-9].*))(?=(.*[a-z].*))(?=(.*[A-Z].*)).{8,}$");
        }
    }
}