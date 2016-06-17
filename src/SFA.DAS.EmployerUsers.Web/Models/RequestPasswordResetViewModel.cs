using System.Collections.Generic;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RequestPasswordResetViewModel
    {
        public string Email { get; set; }

        public bool ResetCodeSent { get; set; }

        public Dictionary<string, string> ErrorDictionary { get; set; }
    }
}