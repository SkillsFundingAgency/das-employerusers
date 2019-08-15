namespace SFA.DAS.EmployerUsers.Web.Models
{
    public class ChangeEmailViewModel : ViewModelBase, ISignedInUserModel
    {
        public string NewEmailAddress { get; set; }
        public string ConfirmEmailAddress { get; set; }

        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string ReturnUrl { get; set; }
        public string NewEmailAddressError => GetErrorMessage(nameof(NewEmailAddress));
        public string ConfirmEmailAddressError => GetErrorMessage(nameof(ConfirmEmailAddress));

    }
}