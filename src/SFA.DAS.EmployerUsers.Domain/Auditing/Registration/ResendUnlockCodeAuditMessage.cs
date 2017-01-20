namespace SFA.DAS.EmployerUsers.Domain.Auditing.Registration
{
    public class ResendUnlockCodeAuditMessage : EmployerUsersAuditMessage
    {
        public ResendUnlockCodeAuditMessage(string email)
        {
            Category = "ACCOUNT_UNLOCK_CODE";
            Description = $"User {email} has requested account unlock code";
        }
    }
}
