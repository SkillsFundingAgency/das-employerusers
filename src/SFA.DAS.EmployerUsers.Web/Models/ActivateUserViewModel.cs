namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class ActivateUserViewModel
    {
        public string AccessCode { get; set; }
        public string UserId { get; set; }
        public bool Valid { get; set; }
        public string ReturnUrl { get; set; }
    }
}