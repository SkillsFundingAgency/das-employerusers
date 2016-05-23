namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public interface IOwinWrapper
    {
        void IssueLoginCookie(string id, string displayName);
    }
}