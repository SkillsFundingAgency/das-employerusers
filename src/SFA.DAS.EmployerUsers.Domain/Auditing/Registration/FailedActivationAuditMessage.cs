using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Registration
{
    public class FailedActivationAuditMessage : EmployerUsersAuditMessage
    {
        public FailedActivationAuditMessage(User user, string enteredAccessCode)
        {
            AffectedEntity = new Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
            Category = "FAILED_ACTIVATION";
            Description = $"User {user.Email} (id: {user.Id}) failed to activate their account using code '{enteredAccessCode}'";
        }
    }
}
