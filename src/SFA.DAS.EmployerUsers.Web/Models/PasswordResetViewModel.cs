using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class PasswordResetViewModel : ViewModelBase
    {
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }
        [AllowHtml]
        public string Password { get; set; }
        [AllowHtml]
        public string ConfirmPassword { get; set; }
        public string ConfirmPasswordError => GetErrorMessage(nameof(ConfirmPassword));
        public string PasswordResetCodeError => GetErrorMessage(nameof(PasswordResetCode));
        public string PasswordError => GetErrorMessage(nameof(Password));
        public string ReturnUrl { get; set; }
        public int UnlockCodeLength { get; set; }
    }
}