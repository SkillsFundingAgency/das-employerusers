namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class PasswordResetViewModel : ViewModelBase
    {
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ConfirmPasswordError => GetErrorMessage(nameof(ConfirmPassword));
        public string PasswordResetCodeError => GetErrorMessage(nameof(PasswordResetCode));
        public string PasswordError => GetErrorMessage(nameof(Password));
    }
}