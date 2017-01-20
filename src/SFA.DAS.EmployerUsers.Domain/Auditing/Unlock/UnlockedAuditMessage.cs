using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Unlock
{
    public class UnlockedAuditMessage : EmployerUsersAuditMessage
    {
        public UnlockedAuditMessage(User user)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "UNLOCK";
            Description = $"User {user.Email} (id: {user.Id}) unlocked their account";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromBool(nameof(user.IsLocked), user.IsLocked),
                PropertyUpdate.FromInt(nameof(user.FailedLoginAttempts), user.FailedLoginAttempts)
            };
        }
    }
}
