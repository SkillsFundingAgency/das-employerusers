using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using MediatR;
using Microsoft.Owin;
using SFA.DAS.EmployerUsers.ApplicationLayer.Queries.IsUserActive;

namespace SFA.DAS.EmployerUsers.Web.Authentication
{
    public class UserService : UserServiceBase
    {
        private readonly IMediator _mediator;
        private readonly OwinContext _owinContext;

        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public UserService(OwinEnvironmentService owinEnvironment, IMediator mediator)
            : this(mediator)
        {
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
        public override async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = await _mediator.SendAsync(new IsUserActiveQuery());
        }
        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            return base.GetProfileDataAsync(context);
        }
    }
}