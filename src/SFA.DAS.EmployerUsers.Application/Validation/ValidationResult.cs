using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerUsers.Application.Validation
{
    public class ValidationResult
    {
        public Dictionary<string,string> ValidationDictionary { get; set; }

        public bool IsValid()
        {
            if (ValidationDictionary == null)
            {
                return true;
            }

            return !ValidationDictionary.Any();
        }
    }
}