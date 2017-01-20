using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Unlock
{
    public class FailedUnlockAuditMessage : EmployerUsersAuditMessage
    {
        public FailedUnlockAuditMessage(User user, string code)
        {
            Category = "UNLOCK";
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Description = $"User {user.Email} (id: {user.Id}) failed to unlock their account with code '{code}'";
        }
    }
}
