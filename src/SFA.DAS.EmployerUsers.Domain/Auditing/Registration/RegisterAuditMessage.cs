using System.Collections.Generic;
using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Registration
{
    public class RegisterAuditMessage : EmployerUsersAuditMessage
    {
        public RegisterAuditMessage(User user)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "CREATED";
            Description = $"User registered with email address {user.Email}";
            ChangedProperties = new List<PropertyUpdate>
            {
                PropertyUpdate.FromString(nameof(user.FirstName), user.FirstName),
                PropertyUpdate.FromString(nameof(user.LastName), user.LastName),
                PropertyUpdate.FromString(nameof(user.Email), user.Email),
                PropertyUpdate.FromString(nameof(user.Password), PasswordAuditValue),
                PropertyUpdate.FromString(nameof(user.Salt), SaltAuditValue),
                PropertyUpdate.FromString(nameof(user.PasswordProfileId), user.PasswordProfileId),
                PropertyUpdate.FromBool(nameof(user.IsActive), user.IsActive)
            };
            for (var i = 0; i < user.SecurityCodes.Length; i++)
            {
                var code = user.SecurityCodes[i];
                ChangedProperties.Add(PropertyUpdate.FromString($"{nameof(user.SecurityCodes)}[{i}].{nameof(code.Code)}", code.Code));
                ChangedProperties.Add(PropertyUpdate.FromString($"{nameof(user.SecurityCodes)}[{i}].{nameof(code.CodeType)}", code.CodeType.ToString()));
                ChangedProperties.Add(PropertyUpdate.FromDateTime($"{nameof(user.SecurityCodes)}[{i}].{nameof(code.ExpiryTime)}", code.ExpiryTime));
            }
        }
    }
}
