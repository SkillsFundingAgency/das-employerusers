using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Login
{
    public class FailedLoginAuditMessage : EmployerUsersAuditMessage
    {
        public FailedLoginAuditMessage(string emailAddress, User user)
        {
            Category = "FAILED_LOGIN";
            if (user != null)
            {
                AffectedEntity = new Audit.Types.Entity
                {
                    Type = UserTypeName,
                    Id = user.Id
                };
                Description = $"User {user.Email} (id: {user.Id})attempted to login with the incorrect password";
                ChangedProperties = new List<PropertyUpdate>
                {
                    PropertyUpdate.FromInt(nameof(user.FailedLoginAttempts), user.FailedLoginAttempts)
                };
            }
            else
            {
                Description = $"Attempt to login with email address {emailAddress} failed as no account exists";
            }
        }
    }
}
