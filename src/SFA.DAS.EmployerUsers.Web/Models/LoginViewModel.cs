using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class LoginViewModel : ViewModelBase
    {
        public string EmailAddress { get; set; }
        [AllowHtml]
        public string Password { get; set; }
        public string OriginatingAddress { get; set; }
        
        public string ReturnUrl { get; set; }
        public string PasswordError => GetErrorMessage(nameof(Password));
        public string EmailAddressError => GetErrorMessage(nameof(EmailAddress));
        public string ClientId { get; set; }
    }
}