namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class ActivateUserViewModel : ViewModelBase, ISignedInUserModel
    {
        public string AccessCode { get; set; }
        public string UserId { get; set; }
        public string ReturnUrl { get; set; }
    }
}