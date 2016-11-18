namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class LoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string OriginatingAddress { get; set; }

        public bool InvalidLoginAttempt { get; set; }
        public string ReturnUrl { get; set; }
    }
}