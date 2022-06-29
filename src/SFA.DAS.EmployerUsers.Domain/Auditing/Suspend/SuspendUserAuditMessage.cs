using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Suspend
{
    public class SuspendUserAuditMessage : EmployerUsersAuditMessage
    {
        public SuspendUserAuditMessage(User user, ChangedByUserInfo changedByUserInfo)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "UPDATE";
            Description = $"User {user.Email} (id: {user.Id}) has been suspended by {changedByUserInfo.Email} (id: {changedByUserInfo.UserId})";
        }
    }
}
