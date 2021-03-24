using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Suspend
{
    public class ResumeUserAuditMessage : EmployerUsersAuditMessage
    {
        public ResumeUserAuditMessage(User user)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "UPDATE";
            Description = $"User {user.Email} (id: {user.Id}) has been re-activated after suspension";
        }
    }
}
