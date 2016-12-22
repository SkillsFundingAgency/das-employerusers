using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using Microsoft.Owin;

namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public class OwinWrapper : IOwinWrapper
    {
        private readonly IOwinContext _owinContext;

        public OwinWrapper(IOwinContext owinContext)
        {
            _owinContext = owinContext;
        }

        public SignInMessage GetSignInMessage(string id)
        {
            return _owinContext.Environment.GetSignInMessage(id);
        }
        public void IssueLoginCookie(string id, string displayName)
        {
            var env = _owinContext.Environment;
            ClearSignInMessageCookie();
            env.IssueLoginCookie(new AuthenticatedLogin
            {
                Subject = id,
                Name = displayName
            });
        }

        public void ClearSignInMessageCookie()
        {
            foreach (var cookie in _owinContext.Request.Cookies)
            {
                if (cookie.Key.ToLower().StartsWith("signinmessage"))
                {
                    _owinContext.Response.Cookies.Delete(cookie.Key);
                }
            }
        }

        public void RemovePartialLoginCookie()
        {
            _owinContext.Environment.RemovePartialLoginCookie();
        }
    }
}