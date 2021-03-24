using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Suspend
{
    public class SuspendUserAuditMessage : EmployerUsersAuditMessage
    {
        public SuspendUserAuditMessage(User user)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "UPDATE";
            Description = $"User {user.Email} (id: {user.Id}) has been suspended";
        }
    }
}
