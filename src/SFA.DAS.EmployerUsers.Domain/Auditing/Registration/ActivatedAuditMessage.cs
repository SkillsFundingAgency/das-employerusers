using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Registration
{
    public class ActivatedAuditMessage : EmployerUsersAuditMessage
    {
        public ActivatedAuditMessage(User user)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "ACTIVATED";
            Description = $"User {user.Email} (id: {user.Id}) activated their account";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromBool(nameof(user.IsActive), user.IsActive)
            };
        }
    }
}
