using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Login
{
    public class PasswordResetCodeAuditMessage : EmployerUsersAuditMessage
    {
        public PasswordResetCodeAuditMessage(User user)
        {
            //Get the latest security code 
            var securityCode = user.SecurityCodes.OrderByDescending(x => x.ExpiryTime).FirstOrDefault();

            Category = "PASSWORD_RESET_CODE";
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Description = $"User {user.Email} (id: {user.Id}) has reset their security code";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromString("SecurityCode", securityCode?.Code)
            };
        }
    }
}
