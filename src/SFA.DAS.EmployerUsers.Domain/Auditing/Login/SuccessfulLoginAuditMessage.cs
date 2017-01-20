namespace SFA.DAS.EmployerUsers.Domain.Auditing.Login
{
    public class SuccessfulLoginAuditMessage : EmployerUsersAuditMessage
    {
        public SuccessfulLoginAuditMessage(User user)
        {
            Category = "SUCCESSFUL_LOGIN";
            Description = $"User {user.Email} (id: {user.Id}) logged in";
            AffectedEntity = new Audit.Types.Entity
            {
                Type = UserTypeName,
                Id = user.Id
            };
        }
    }
}
