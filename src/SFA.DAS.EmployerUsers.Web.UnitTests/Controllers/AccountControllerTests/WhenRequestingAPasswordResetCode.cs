using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenRequestingAPasswordResetCode : ControllerTestBase
    {
        private const string Id = "UNIT_TESTS";
        private const string ReturnUrl = "http://unittests.local";

        private Mock<AccountOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountController _controller;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            _orchestrator = new Mock<AccountOrchestrator>();
            _orchestrator.Setup(o => o.ResetPassword(It.IsAny<PasswordResetViewModel>())).ReturnsAsync(new PasswordResetViewModel());

            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(w => w.GetSignInMessage(Id))
                .Returns(new SignInMessage
                {
                    ReturnUrl = ReturnUrl
                });

            _controller = new AccountController(_orchestrator.Object, _owinWrapper.Object, null);
            _controller.ControllerContext = _controllerContext.Object;
        }
        
    }
}
