namespace SFA.DAS.EmployerUsers.Application.Queries.IsPasswordResetValid
{
    public class PasswordResetCodeResponse
    {
        public bool IsValid { get; set; }   

        public bool HasExpired { get; set; }
    }
}