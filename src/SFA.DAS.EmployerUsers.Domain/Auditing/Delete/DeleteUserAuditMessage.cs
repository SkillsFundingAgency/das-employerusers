using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Delete
{
    public class DeleteUserAuditMessage : EmployerUsersAuditMessage
    {
        public DeleteUserAuditMessage(User user)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "DELETE";
            Description = $"User {user.Email} (id: {user.Id}) has been deleted as they did not complete their registration";
        }
    }
}
