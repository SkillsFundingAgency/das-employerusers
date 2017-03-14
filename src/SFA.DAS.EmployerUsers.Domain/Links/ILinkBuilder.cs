namespace SFA.DAS.EmployerUsers.Domain.Links
{
    public interface ILinkBuilder
    {
        string GetRegistrationUrl();
        string GetForgottenPasswordUrl(string hashedUserId);
    }
}