namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RequestPasswordResetViewModel : ViewModelBase
    {
        public string SignInId { get; set; }
        public string ClientId { get; set; }
        public string ReturnUrl { get; set; }

        public string Email { get; set; }
        public bool ResetCodeSent { get; set; }
        
        public string EmailError => GetErrorMessage(nameof(Email));
    }
}