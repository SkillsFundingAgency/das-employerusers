using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class RequestPasswordResetViewModel : ViewModelBase
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public bool ResetCodeSent { get; set; }

        public string EmailError => GetErrorMessage(nameof(Email));

    }
}