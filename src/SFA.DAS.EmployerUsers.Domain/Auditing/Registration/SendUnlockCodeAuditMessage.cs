using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Registration
{
    public class SendUnlockCodeAuditMessage : EmployerUsersAuditMessage
    {
        public SendUnlockCodeAuditMessage(User user, SecurityCode code)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "UNLOCK";
            Description = $"User {user.Email} (id: {user.Id}) has been sent an unlock code of {code.Code}";
        }
    }
}
