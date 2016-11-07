namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RequestPasswordResetViewModel : ViewModelBase
    {
        public string Email { get; set; }

        public bool ResetCodeSent { get; set; }

        public string EmailError => GetErrorMessage(nameof(Email));

    }
}