using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RegisterViewModel :ViewModelBase
    {
        public string FirstName { get; set; }

        public string FirstNameError => GetErrorMessage(nameof(FirstName));

        public string LastName { get; set; }

        public string LastNameError => GetErrorMessage(nameof(LastName));

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        
        
        public string EmailError => GetErrorMessage(nameof(Email));
        public string PasswordError => GetErrorMessage(nameof(Password));
        public string ConfirmPasswordError => GetErrorMessage(nameof(ConfirmPassword));
        public string PasswordComplexityError => GetErrorMessage("PasswordComplexity");
        public string PasswordsDontMatchError => GetErrorMessage("PasswordNotMatch");
    }
}