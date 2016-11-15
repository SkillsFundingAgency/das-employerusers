namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class ConfirmChangeEmailViewModel : ViewModelBase
    {
        public string SecurityCode { get; set; }
        public string Password { get; set; }

        public string UserId { get; set; }
        public string ReturnUrl { get; set; }
    }
}