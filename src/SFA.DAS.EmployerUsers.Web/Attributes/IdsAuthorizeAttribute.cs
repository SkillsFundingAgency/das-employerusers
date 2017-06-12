

using System.Web;
using System.Web.Mvc;
using IdentityServer3.Core.Extensions;
using MediatR;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty;
using SFA.DAS.EmployerUsers.Web.Models;
using StructureMap.Attributes;

namespace SFA.DAS.EmployerUsers.Web.Attributes
{
    public class IdsAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var clientId = filterContext.HttpContext.Request.QueryString["clientId"];

            var returnUrl = RelyingPartyLoginUrlModel.RelyingPartyDictionary[clientId];
            

            var redirectUri = HttpUtility.UrlEncode(returnUrl);
            var url = $"https://{filterContext.HttpContext.Request.Url.Authority}/identity/connect/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope=openid%20profile";

            filterContext.Result = new RedirectResult(url);
            
        }
    }
}