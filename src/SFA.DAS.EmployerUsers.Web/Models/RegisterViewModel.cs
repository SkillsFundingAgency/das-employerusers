using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool Valid { get; set; }
        public Dictionary<string,string> ErrorDictionary { get; set; } 
    }
}