using System.Web.Mvc;

namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class ConfirmChangeEmailViewModel : ViewModelBase, ISignedInUserModel
    {
        public string SecurityCode { get; set; }
        [AllowHtml]
        public string Password { get; set; }

        public string UserId { get; set; }
        public string ReturnUrl { get; set; }
    }
}