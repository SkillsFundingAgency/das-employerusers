using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityServer3.Core.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenResetingMyPassword : ControllerTestBase
    {
        private const string Id = "UNIT_TESTS";
        private const string ReturnUrl = "http://unittests.local";

        private Mock<AccountOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountController _controller;
        private Mock<IConfigurationService> _configurationService;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            _configurationService = new Mock<IConfigurationService>();
            _configurationService.Setup(x => x.GetAsync<EmployerUsersConfiguration>())
                .ReturnsAsync(new EmployerUsersConfiguration {IdentityServer = new IdentityServerConfiguration {EmployerPortalUrl = ReturnUrl} });

            _orchestrator = new Mock<AccountOrchestrator>();
            _orchestrator.Setup(o => o.ResetPassword(It.IsAny<PasswordResetViewModel>())).ReturnsAsync(new PasswordResetViewModel());

            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(w => w.GetSignInMessage(Id))
                .Returns(new SignInMessage
                {
                    ReturnUrl = ReturnUrl
                });

            _controller = new AccountController(_orchestrator.Object, _owinWrapper.Object, _configurationService.Object);
            _controller.ControllerContext = _controllerContext.Object;
        }

        [Test]
        public async Task ThenTheOrchestratorIsCalled()
        {
            //Act
            await _controller.ResetPassword(new PasswordResetViewModel());

            //Assert
            _orchestrator.Verify(x=>x.ResetPassword(It.IsAny<PasswordResetViewModel>()),Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsRedirectedIfTheModelIsValid()
        {
            //Act
            var actual = await _controller.ResetPassword(new PasswordResetViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirect = actual as RedirectResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual(ReturnUrl, actualRedirect.Url);
        }


        [Test]
        public async Task ThenTheUserIsReturnedToThePasswordResetViewIfTheModelIsNotValid()
        {
            //Arrange
            _orchestrator.Setup(o => o.ResetPassword(It.IsAny<PasswordResetViewModel>())).ReturnsAsync(new PasswordResetViewModel {ErrorDictionary = new Dictionary<string, string> { {"someError","some error"} } });

            //Act
            var actual = await _controller.ResetPassword(new PasswordResetViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(actualViewResult.ViewName, "ResetPassword");
        }
    }
}
