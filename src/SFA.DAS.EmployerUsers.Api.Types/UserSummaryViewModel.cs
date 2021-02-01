namespace SFA.DAS.EmployerUsers.Api.Types
{
    public class UserSummaryViewModel : IEmployerUsersResource
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public string Email { get; set; }
        public string Href { get; set; }
        public bool IsSuspended { get; set; }

    }
}
