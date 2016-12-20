namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }

        public string ClientId { get; set; }
        public string ReturnUrl { get; set; }

        public string CurrentPasswordError => GetErrorMessage(nameof(CurrentPassword));
        public string NewPasswordError => GetErrorMessage(nameof(NewPassword));
        public string ConfirmPasswordError => GetErrorMessage(nameof(ConfirmPassword));
    }
}