using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public abstract class ViewModelBase
    {
        protected ViewModelBase()
        {
            ErrorDictionary = new Dictionary<string, string>();
        }

        public bool Valid => !ErrorDictionary.Any();

        public Dictionary<string, string> ErrorDictionary { get; set; }

        public string GeneralError => GetErrorMessage("");

        protected string GetErrorMessage(string propertyName)
        {
            return ErrorDictionary.Any() && ErrorDictionary.ContainsKey(propertyName) ? ErrorDictionary[propertyName] : "";
        }
    }
}