using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Update
{
    public class UpdateUserAuditMessage : EmployerUsersAuditMessage
    {
        public UpdateUserAuditMessage(User oldUser, User newUser)
        {
            Category = "UPDATE";
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = newUser.Id
            };
            Description = $"User {newUser.Email} (id: {newUser.Id}) had been updated";
            ChangedProperties = CalculateDelta(oldUser, newUser);
        }

        private List<PropertyUpdate> CalculateDelta(User oldUser, User newUser)
        {
            var updates = new List<PropertyUpdate>();

            if (newUser.RequiresPasswordReset != oldUser.RequiresPasswordReset)
            {
                updates.Add(PropertyUpdate.FromBool(nameof(newUser.RequiresPasswordReset), newUser.RequiresPasswordReset));
            }

            return updates;
        }
    }
}
