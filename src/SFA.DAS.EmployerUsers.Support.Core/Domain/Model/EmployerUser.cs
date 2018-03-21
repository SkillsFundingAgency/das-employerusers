using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EmployerUsers.Support.Core.Domain.Model
{
    [ExcludeFromCodeCoverage]
    public class EmployerUser : IUser
    {
        public int FailedLoginAttempts { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }

        public UserStatus Status => DetermineStatus();
        public ICollection<AccountDetailViewModel> Accounts { get; set; }
        public string AccountsUri { get; set; }
        private UserStatus DetermineStatus()
        {
            if (IsActive && IsLocked)
                return UserStatus.Locked;
            if (IsActive && IsLocked == false)
                return UserStatus.Active;

            return UserStatus.Unverified;
        }
    }
}