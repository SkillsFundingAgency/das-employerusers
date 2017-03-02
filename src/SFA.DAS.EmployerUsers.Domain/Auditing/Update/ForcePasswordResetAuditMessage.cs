using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Update
{
    public class ForcePasswordResetAuditMessage : EmployerUsersAuditMessage
    {
        public ForcePasswordResetAuditMessage(User user)
        {
            Category = "FORCE_PASSWORD_RESET";
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Description = $"User {user.Email} (id: {user.Id}) has been set to force a password reset";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromBool(nameof(user.RequiresPasswordReset), true)
            };
        }
    }
}
