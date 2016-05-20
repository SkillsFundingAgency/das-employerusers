namespace SFA.DAS.EmployerUsers.Application.Services.Password
{
    public class SecuredPassword
    {
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public string ProfileId { get; set; }
    }
}