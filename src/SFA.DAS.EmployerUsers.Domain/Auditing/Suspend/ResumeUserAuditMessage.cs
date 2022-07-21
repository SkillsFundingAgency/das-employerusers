using SFA.DAS.Audit.Types;
using SFA.DAS.EmployerUsers.Api.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Suspend
{
    public class ResumeUserAuditMessage : EmployerUsersAuditMessage
    {
        public ResumeUserAuditMessage(User user, ChangedByUserInfo changedByUserInfo)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "UPDATE";
            Description = $"User {user.Email} (id: {user.Id}) has been re-activated after suspension by {changedByUserInfo.Email} (id: {changedByUserInfo.UserId})";
        }
    }
}
