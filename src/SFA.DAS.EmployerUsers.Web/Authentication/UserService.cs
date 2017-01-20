using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using MediatR;
using Microsoft.Owin;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public class UserService : UserServiceBase
    {
        private readonly IMediator _mediator;
        private readonly IOwinContext _owinContext;

        public UserService(OwinEnvironmentService owinEnvironment, IMediator mediator)
        {
            _mediator = mediator;
            _owinContext = new OwinContext(owinEnvironment.Environment);
        }

        public override Task PreAuthenticateAsync(PreAuthenticationContext context)
        {
            var id = _owinContext.Request.Query.Get("signin");
            var returnUrl = HttpUtility.UrlEncode(context.SignInMessage.ReturnUrl);
            var clientId = context.SignInMessage.ClientId;
            var url = $"~/employer/login?id={id}&returnUrl={returnUrl}&clientId={clientId}";

            context.AuthenticateResult = new AuthenticateResult(url, (IEnumerable<Claim>)null);
            return Task.FromResult(0);
        }
        public override Task IsActiveAsync(IsActiveContext context)
        {
            var userId = GetUserId(context.Subject);
            if (string.IsNullOrEmpty(userId))
            {
                context.IsActive = false;
                return Task.FromResult<object>(null);
            }

            context.IsActive = true;
            return Task.FromResult<object>(null);
            //context.IsActive = await _mediator.SendAsync(new IsUserActiveQuery { UserId = userId });
        }

        public override async Task SignOutAsync(SignOutContext context)
        {
            context.Subject = new ClaimsPrincipal(new ClaimsIdentity());

            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            var env = _owinContext.Authentication;
            env.SignOut();
            

            await base.SignOutAsync(context);
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = GetUserId(context.Subject);

            var user = await _mediator.SendAsync(new GetUserByIdQuery { UserId = userId });
            if (user == null)
            {
                return;
            }

            var claims = new List<Claim>
            {
                new Claim(DasClaimTypes.Id, user.Id),
                new Claim(DasClaimTypes.Email, user.Email),
                new Claim(DasClaimTypes.GivenName, user.FirstName),
                new Claim(DasClaimTypes.FamilyName, user.LastName),
                new Claim(DasClaimTypes.DisplayName, $"{user.FirstName} {user.LastName}"),
                new Claim(DasClaimTypes.RequiresVerification, (!user.IsActive).ToString())
            };
            context.IssuedClaims = claims;
        }


        private string GetUserId(ClaimsPrincipal subject)
        {
            var claim = subject?.Claims.FirstOrDefault(c => c.Type.Equals(DasClaimTypes.Id));
            if (claim == null)
            {
                claim = subject?.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimTypes.Subject));
            }
            return claim?.Value;
        }
    }
}