namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class ValidatePasswordResetViewModel : ViewModelBase
    {
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ConfrimPasswordError => GetErrorMessage(nameof(ConfirmPassword));
        public string PasswordResetCodeError => GetErrorMessage(nameof(PasswordResetCode));
    }
}