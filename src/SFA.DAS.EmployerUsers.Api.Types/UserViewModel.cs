using System;

namespace SFA.DAS.EmployerUsers.Api.Types
{
    public class UserViewModel : IEmployerUsersResource
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsLocked { get; set; }
        public bool IsSuspended { get; set; }
        public DateTime? LastSuspendedDate { get; set; }
        public string GovUkIdentifier { get; set; }
    }
}
