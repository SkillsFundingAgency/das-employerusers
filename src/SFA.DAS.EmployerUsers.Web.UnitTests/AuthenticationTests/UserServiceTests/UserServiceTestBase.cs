using System.Collections.Generic;
using IdentityServer3.Core.Services;
using MediatR;
using Microsoft.Owin;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Authentication;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.AuthenticationTests.UserServiceTests
{
    public abstract class UserServiceTestBase
    {
        protected Dictionary<string, string[]> _requestQueryString;
        protected OwinEnvironmentService _owinEnvironmentService;
        protected Mock<IMediator> _mediator;
        protected UserService _userService;

        [SetUp]
        public virtual void Arrange()
        {
            _requestQueryString = new Dictionary<string, string[]>();

            _owinEnvironmentService = new OwinEnvironmentService(new Dictionary<string, object>
            {
                { "Microsoft.Owin.Query#dictionary", _requestQueryString }
            });

            _mediator = new Mock<IMediator>();

            _userService = new UserService(_owinEnvironmentService, _mediator.Object);
        }
    }
}
