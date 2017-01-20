using SFA.DAS.Audit.Types;

namespace SFA.DAS.EmployerUsers.Domain.Auditing.Unlock
{
    public class FailedUnlockAuditMessage : EmployerUsersAuditMessage
    {
        public FailedUnlockAuditMessage(User user, string emailAddress, string code)
        {
            Category = "UNLOCK";
            if (user != null)
            {
                AffectedEntity = new Entity
                {
                    Type = UserTypeName,
                    Id = user.Id
                };
                Description = $"User {user.Email} (id: {user.Id}) failed to unlock their account with code '{code}'";
            }
            else
            {
                Description = $"An attempt to unlock an account with email {emailAddress} using code '{code}', however no such account exists";
            }
        }
    }
}
