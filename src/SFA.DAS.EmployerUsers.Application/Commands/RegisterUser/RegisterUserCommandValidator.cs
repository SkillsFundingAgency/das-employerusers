using System.Text.RegularExpressions;

namespace SFA.DAS.EmployerUsers.Application.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : IValidator<RegisterUserCommand>
    {
        public bool Validate(RegisterUserCommand item)
        {
            if (string.IsNullOrWhiteSpace(item?.Email) || string.IsNullOrWhiteSpace(item.FirstName) ||
                string.IsNullOrWhiteSpace(item.LastName) || string.IsNullOrWhiteSpace(item.Password) ||
                string.IsNullOrWhiteSpace(item.ConfirmPassword))
            {
                return false;
            }

            if (CheckPasswordMatchesAtLeastOneUppercaseOneLowercaseOneNumberAndAtLeastEightCharacters(item.Password))
            {
                return false;
            }

            if (!item.ConfirmPassword.Equals(item.Password))
            {
                return false;
            }

            return true;
        }

        private static bool CheckPasswordMatchesAtLeastOneUppercaseOneLowercaseOneNumberAndAtLeastEightCharacters(string password)
        {
            return !Regex.IsMatch(password, @"^(?=(.*[0-9].*))(?=(.*[a-z].*))(?=(.*[A-Z].*)).{8,}$");
        }
    }
}