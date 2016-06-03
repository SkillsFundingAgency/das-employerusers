using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : IValidator<RegisterUserCommand>
    {
        public Dictionary<string,string> Validate(RegisterUserCommand item)
        {
            if (string.IsNullOrWhiteSpace(item?.Email) || string.IsNullOrWhiteSpace(item.FirstName) ||
                string.IsNullOrWhiteSpace(item.LastName) || string.IsNullOrWhiteSpace(item.Password) ||
                string.IsNullOrWhiteSpace(item.ConfirmPassword))
            {
                return new Dictionary<string, string> { { "", "" } };
            }

            if (CheckPasswordMatchesAtLeastOneUppercaseOneLowercaseOneNumberAndAtLeastEightCharacters(item.Password))
            {
                return new Dictionary<string, string> { { "", "" } };
            }

            if (!item.ConfirmPassword.Equals(item.Password))
            {
                return new Dictionary<string, string> { { "", "" } };
            }

            return new Dictionary<string, string>();
        }

        private static bool CheckPasswordMatchesAtLeastOneUppercaseOneLowercaseOneNumberAndAtLeastEightCharacters(string password)
        {
            return !Regex.IsMatch(password, @"^(?=(.*[0-9].*))(?=(.*[a-z].*))(?=(.*[A-Z].*)).{8,}$");
        }
    }
}