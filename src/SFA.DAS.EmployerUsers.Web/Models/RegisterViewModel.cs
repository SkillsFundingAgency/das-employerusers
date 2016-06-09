using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            ErrorDictionary = new Dictionary<string, string>();    
        }

        public string FirstName { get; set; }


        public string LastName { get; set; }


        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool HasAcceptedTermsAndConditions { get; set; }


        public bool Valid => !ErrorDictionary.Any();
        public Dictionary<string,string> ErrorDictionary { get; set; }
        public string FirstNameError => GetErrorMessage(nameof(FirstName));
        public string LastNameError => GetErrorMessage(nameof(LastName));
        public string EmailError => GetErrorMessage(nameof(Email));
        public string PasswordError => GetErrorMessage(nameof(Password));
        public string ConfirmPasswordError => GetErrorMessage(nameof(ConfirmPassword));
        public string PasswordComplexityError => GetErrorMessage("PasswordComplexity");
        public string PasswordsDontMatchError => GetErrorMessage("PasswordNotMatch");
        public string HasAcceptedTermsAndConditionsError => GetErrorMessage(nameof(HasAcceptedTermsAndConditions));

        private string GetErrorMessage(string propertyName)
        {
            return ErrorDictionary.Any() && ErrorDictionary.ContainsKey(propertyName) ? ErrorDictionary[propertyName] : "";
        }
    }
}