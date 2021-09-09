using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityServer3.Core.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Models.SFA.DAS.EAS.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenEnteringPasswordResetCode : ControllerTestBase
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
            _orchestrator.Setup(o => o.ValidateResetCode(It.IsAny<EnterResetCodeViewModel>())).ReturnsAsync(new OrchestratorResponse<EnterResetCodeViewModel>
            {
                Data = new EnterResetCodeViewModel()
            });

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
            // Act
            await _controller.EnterResetCode(new EnterResetCodeViewModel());

            // Assert
            _orchestrator.Verify(x=>x.ValidateResetCode(It.IsAny<EnterResetCodeViewModel>()), Times.Once);
        }
        
        [Test]
        public async Task ThenTheUserIsReturnedToTheEnterResetCodeViewIfTheModelIsNotValid()
        {
            // Arrange
            _orchestrator
                .Setup(o => o.ValidateResetCode(It.IsAny<EnterResetCodeViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<EnterResetCodeViewModel>
                {
                    FlashMessage = new FlashMessageViewModel {ErrorMessages = new Dictionary<string, string> { {"someError","some error"} } }
                });

            // Act
            var actual = await _controller.EnterResetCode(new EnterResetCodeViewModel());

            // Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(actualViewResult.ViewName, "EnterResetCode");
        }

        [Test]
        public async Task ThenTheUserIsShownTheResetPasswordViewIfTheModelIsValid()
        {
            // Arrange
            _orchestrator
                .Setup(o => o.ValidateResetCode(It.IsAny<EnterResetCodeViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<EnterResetCodeViewModel>
                {
                    Data = new EnterResetCodeViewModel(),
                    FlashMessage = new FlashMessageViewModel { ErrorMessages = new Dictionary<string, string> () }
                });

            // Act
            var actual = await _controller.EnterResetCode(new EnterResetCodeViewModel());

            // Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(actualViewResult.ViewName, "ResetPassword");
        }

        [Test]
        public async Task ThenTheUserIsShownTheInvalidResetCodeViewIfAnExceededLimitPasswordResetCodeExceptionIsReturned()
        {
            // Arrange
            _orchestrator.Setup(x => x.ValidateResetCode(It.IsAny<EnterResetCodeViewModel>())).ReturnsAsync(new OrchestratorResponse<EnterResetCodeViewModel>
            {
                Exception = new ExceededLimitPasswordResetCodeException()
            });

            // Act
            var actual = await _controller.EnterResetCode(new EnterResetCodeViewModel());

            // Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual(actualViewResult.ViewName, "InvalidResetCode");
        }
    }
}
