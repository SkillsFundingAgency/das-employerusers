using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class EnterResetCodeViewModel : ViewModelBase
    {
        public string ClientId { get; set; }
        public string Email { get; set; }
        public string PasswordResetCode { get; set; }
        public string PasswordResetCodeError => GetErrorMessage(nameof(PasswordResetCode));
        public string ReturnUrl { get; set; }
        public int UnlockCodeLength { get; set; }
    }
}