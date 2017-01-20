using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Registration
{
    public class ResendActivationCodeAuditMessage : EmployerUsersAuditMessage
    {
        public ResendActivationCodeAuditMessage(User user)
        {
            var activationCode = user.SecurityCodes.Where(x => x.CodeType == SecurityCodeType.AccessCode)
                .OrderByDescending(x => x.ExpiryTime)
                .FirstOrDefault();

            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "RESEND_ACTIVATION_CODE";
            Description = $"User {user.Email} (id: {user.Id}) has request activation code {activationCode?.Code} be resent";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromString(nameof(user.SecurityCodes), activationCode?.Code)
            };
        }
    }
}
