namespace SFA.DAS.EmployerUsers.WebClientComponents
{
    public static class DasClaimTypes
    {
        private const string BaseUrl = "http://das/employer/identity/claims/";

        public const string Id = BaseUrl + "id";
        public const string Email = BaseUrl + "email_address";
        public const string GivenName = BaseUrl + "given_name";
        public const string FamilyName = BaseUrl + "family_name";
        public const string DisplayName = BaseUrl + "display_name";
        public const string RequiresVerification = BaseUrl + "requires_verification";
    }
}
