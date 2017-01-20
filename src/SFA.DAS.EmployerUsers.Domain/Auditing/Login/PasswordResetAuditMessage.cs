using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Login
{
    public class PasswordResetAuditMessage : EmployerUsersAuditMessage
    {
        public PasswordResetAuditMessage(User user)
        {
            Category = "PASSWORD_RESET";
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Description = $"User {user.Email} (id: {user.Id}) has reset their password";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromString(nameof(user.Password), PasswordAuditValue),
                PropertyUpdate.FromString(nameof(user.Salt), SaltAuditValue)
            };
        }
    }
}
