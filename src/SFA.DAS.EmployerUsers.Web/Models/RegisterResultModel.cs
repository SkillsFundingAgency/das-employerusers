using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RegisterResultModel
    {
        public Dictionary<string,string> ErrorDictionary { get; set; }

        public RegisterResultModel()
        {
            ErrorDictionary = new Dictionary<string, string>();
        }

        public bool IsValid() => !ErrorDictionary.Any();
        
    }
}