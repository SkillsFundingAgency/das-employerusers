using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Login
{
    public class AccountLockedAuditMessage : EmployerUsersAuditMessage
    {
        public AccountLockedAuditMessage(User user)
        {
            Category = "ACCOUNT_LOCKED";
            Description = $"User {user.Email} (id: {user.Id}) has exceeded the limit of failed logins and the account has been locked";
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromInt(nameof(user.FailedLoginAttempts), user.FailedLoginAttempts),
                PropertyUpdate.FromBool(nameof(user.IsLocked), user.IsLocked)
            };
        }
    }
}
