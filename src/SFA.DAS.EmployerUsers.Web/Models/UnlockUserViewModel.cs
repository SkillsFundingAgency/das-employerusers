namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class UnlockUserViewModel : ViewModelBase
    {
        public string UnlockCode { get; set; }
        public string Email { get; set; }
        public bool UnlockCodeExpired { get; set; }
        public string UnlockCodeError => GetErrorMessage(nameof(UnlockCode));
        public string EmailError => GetErrorMessage(nameof(Email));
        public bool UnlockCodeSent { get; set; }
        public string ReturnUrl { get; set; }
        public int UnlockCodeLength { get; set; }
    }
}