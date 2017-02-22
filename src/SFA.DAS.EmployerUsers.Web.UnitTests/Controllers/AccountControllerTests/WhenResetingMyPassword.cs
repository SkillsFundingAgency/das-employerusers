using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityServer3.Core.Models;
using Moq;
using NUnit.Framework;
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
        private const string EmployerPortalReturnUrl = "http://employerportal.returnurl.local";

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
            _owinWrapper.Setup(x => x.GetIdsReturnUrl()).Returns(ReturnUrl);

            var identityServerConfiguration = new IdentityServerConfiguration { EmployerPortalUrl = EmployerPortalReturnUrl };
            _controller = new AccountController(_orchestrator.Object, _owinWrapper.Object, identityServerConfiguration, _logger.Object);
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

        [Test]
        public async Task ThenIfTheModelReturnUrlIsNotEmptyWeRedirectToIt()
        {
            //Arrange
            var expectedReturnUrl = "http://test.url";
            _orchestrator.Setup(o => o.ResetPassword(It.IsAny<PasswordResetViewModel>())).ReturnsAsync(new PasswordResetViewModel {ReturnUrl = expectedReturnUrl});

            //Act
            var actual = await _controller.ResetPassword(new PasswordResetViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirect = actual as RedirectResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual(expectedReturnUrl,actualRedirect.Url);
        }


        [Test]
        public async Task ThenIfTheModelReturnUrlIsEmptyWeRedirectToTheReturnUrlInTheCookie()
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
        public async Task ThenIfTheModelReturnUrlIsEmptyAndTheReturnUrlInTheCookieIsEmptyWeRedirectToPortalUrl()
        {
            //Arrange
            _owinWrapper.Setup(x => x.GetIdsReturnUrl()).Returns(string.Empty);

            //Act
            var actual = await _controller.ResetPassword(new PasswordResetViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var actualRedirect = actual as RedirectResult;
            Assert.IsNotNull(actualRedirect);
            Assert.AreEqual(EmployerPortalReturnUrl, actualRedirect.Url);
        }
    }
}
