using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Login
{
    public class ResendUnlockCodeAuditMessage : EmployerUsersAuditMessage
    {
        public ResendUnlockCodeAuditMessage(string email)
        {
            Category = "ACCOUNT_UNLOCK_CODE";
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = email
            };
            Description = $"User {email} has requested account unlock code";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromString("UnlockCode", "Unknown value")
            };
        }
    }
}
