using IdentityServer3.Core.Models;

namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public interface IOwinWrapper
    {
        SignInMessage GetSignInMessage(string id);
        void IssueLoginCookie(string id, string displayName);
        void RemovePartialLoginCookie();
    }
}