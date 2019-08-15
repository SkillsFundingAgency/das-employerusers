using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class ChangePasswordViewModel : ViewModelBase, ISignedInUserModel
    {
        public string UserId { get; set; }
        [AllowHtml]
        public string CurrentPassword { get; set; }
        [AllowHtml]
        public string NewPassword { get; set; }
        [AllowHtml]
        public string ConfirmPassword { get; set; }

        public string ClientId { get; set; }
        public string ReturnUrl { get; set; }

        public string CurrentPasswordError => GetErrorMessage(nameof(CurrentPassword));
        public string NewPasswordError => GetErrorMessage(nameof(NewPassword));
        public string ConfirmPasswordError => GetErrorMessage(nameof(ConfirmPassword));
    }
}