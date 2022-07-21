namespace SFA.DAS.EmployerUsers.Api.Types
{
    public class ChangedByUserInfo
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        public ChangedByUserInfo(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
