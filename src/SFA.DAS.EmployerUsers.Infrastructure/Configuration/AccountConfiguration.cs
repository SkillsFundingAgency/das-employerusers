namespace SFA.DAS.EmployerUsers.Infrastructure.Configuration
{
    public class AccountConfiguration
    {
        public string ActivePasswordProfileId { get; set; }

        public int AllowedFailedLoginAttempts { get; set; }
        public int UnlockCodeLength { get; set; }
    }
}