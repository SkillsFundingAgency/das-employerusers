using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RequestPasswordResetViewModel
    {
        public RequestPasswordResetViewModel()
        {
            ErrorDictionary = new Dictionary<string, string>();
        }

        public string Email { get; set; }

        public bool ResetCodeSent { get; set; }

        public Dictionary<string, string> ErrorDictionary { get; set; }
    }
}