namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class LoginViewModel : ViewModelBase
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string OriginatingAddress { get; set; }
        
        public string ReturnUrl { get; set; }
        public string PasswordError => GetErrorMessage(nameof(Password));
        public string EmailAddressError => GetErrorMessage(nameof(EmailAddress));
    }
}