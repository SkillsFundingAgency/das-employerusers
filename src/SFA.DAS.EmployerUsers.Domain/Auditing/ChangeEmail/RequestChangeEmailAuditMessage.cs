using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.ChangeEmail
{
    public class RequestChangeEmailAuditMessage : EmployerUsersAuditMessage
    {
        public RequestChangeEmailAuditMessage(User user, SecurityCode code)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "CHANGE_EMAIL";
            Description = $"User {user.Email} (id: {user.Id}) has requested to change their email to {code.PendingValue}. They have been issued code {code.Code}";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromString("SecurityCodes.Code", code.Code),
                PropertyUpdate.FromDateTime("SecurityCodes.ExpiryTime", code.ExpiryTime)
            };
        }
    }
}
