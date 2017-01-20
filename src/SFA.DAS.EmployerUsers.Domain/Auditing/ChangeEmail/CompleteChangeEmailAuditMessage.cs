using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.ChangeEmail
{
    public class CompleteChangeEmailAuditMessage : EmployerUsersAuditMessage
    {
        public CompleteChangeEmailAuditMessage(User user, string oldEmail)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "UPDATE";
            Description = $"User {user.Email} completed changing their email address from {oldEmail}";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromString(nameof(user.Email), user.Email)
            };
        }
    }
}
