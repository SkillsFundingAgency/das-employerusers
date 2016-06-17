namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class PasswordResetViewModel
    {
        public string Email { get; set; }

        public bool HasExpired { get; set; }

        public bool IsValid { get; set; }
        public string PasswordResetCode { get; set; }
    }
}