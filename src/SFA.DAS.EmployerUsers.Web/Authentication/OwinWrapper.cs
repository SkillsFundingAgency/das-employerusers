using System;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Security;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using Microsoft.Owin;
using Newtonsoft.Json;

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
            env.IssueLoginCookie(new AuthenticatedLogin
            {
                Subject = id,
                Name = displayName
            });
        }
        
        public void SetIdsContext(string returnUrl, string clientId)
        {
            var value = new IdsContext() { ReturnUrl = returnUrl, ClientId = clientId };
            var cookieOptions = new CookieOptions() { Secure = true, HttpOnly = true};

            var encCookie = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value))));
            _owinContext.Response.Cookies.Append(IdsContext.CookieName, encCookie, cookieOptions); ;
        }

        public string GetIdsReturnUrl()
        {
            var cookie = IdsContext.ReadFrom(_owinContext.Request.Cookies[IdsContext.CookieName]);
            return cookie.ReturnUrl;
        }



        public string GetIdsClientId()
        {
            var cookie = IdsContext.ReadFrom(_owinContext.Request.Cookies[IdsContext.CookieName]);
            return cookie.ClientId;
        }

        public void RemovePartialLoginCookie()
        {
            _owinContext.Environment.RemovePartialLoginCookie();
            

        }

        public void SignoutUser()
        {
            _owinContext.Authentication.SignOut();

            _owinContext.Authentication.User = new ClaimsPrincipal(new ClaimsIdentity(string.Empty));

            _owinContext.Request.User = new ClaimsPrincipal(new ClaimsIdentity(string.Empty));

            HttpContext.Current.User = new ClaimsPrincipal(new ClaimsIdentity(string.Empty));
            
        }
    }
}