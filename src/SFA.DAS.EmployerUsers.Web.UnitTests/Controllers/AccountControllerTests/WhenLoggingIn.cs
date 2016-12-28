using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityServer3.Core.Models;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenLoggingIn : ControllerTestBase
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
            _orchestrator.Setup(o => o.Login(It.IsAny<LoginViewModel>())).Returns(Task.FromResult(new OrchestratorResponse<LoginResultModel> { Data = new LoginResultModel { Success = false }}));

            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(w => w.GetSignInMessage(Id))
                .Returns(new SignInMessage
                {
                    ReturnUrl = ReturnUrl
                });



            _controller = new AccountController(_orchestrator.Object, _owinWrapper.Object, null);
            _controller.ControllerContext = _controllerContext.Object;
        }

        [Test]
        public async Task ThenItShouldReturnViewIfUnsuccessful()
        {
            // Act
            var actual = await _controller.Login(Id, new LoginViewModel());

            // Assert
            Assert.IsInstanceOf<ViewResult>(actual);
        }
        
        [Test]
        public async Task ThenItShouldReturnARedirectToReturnUrlIfSuccessful()
        {
            // Arrange
            _orchestrator.Setup(o => o.Login(It.IsAny<LoginViewModel>())).Returns(Task.FromResult(new OrchestratorResponse<LoginResultModel> { Data = new LoginResultModel { Success = true }}));

            // Act
            var actual = await _controller.Login(Id, new LoginViewModel());

            // Assert
            Assert.IsInstanceOf<RedirectResult>(actual);
            Assert.AreEqual(ReturnUrl, ((RedirectResult)actual).Url);
        }

        [Test]
        public async Task ThenItShouldReturnARedirectToConfirmationIfSuccessfulButRequiresActivation()
        {
            // Arrange
            _orchestrator.Setup(o => o.Login(It.IsAny<LoginViewModel>())).Returns(
                Task.FromResult(new OrchestratorResponse<LoginResultModel> { Data = new LoginResultModel { Success = true, RequiresActivation = true }}));

            // Act
            var actual = await _controller.Login(Id, new LoginViewModel()) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.RouteValues.Any(v => v.Key == "action" && (string)v.Value == "Confirm"));
        }

        [Test]
        public async Task ThenItShouldReturnARedirectToUnlockIfUnsuccessfulAndAccountIsLocked()
        {
            // Arrange
            _orchestrator.Setup(o => o.Login(It.IsAny<LoginViewModel>())).Returns(
                Task.FromResult(new OrchestratorResponse<LoginResultModel> { Data = new LoginResultModel { Success = false, AccountIsLocked = true }}));

            // Act
            var actual = await _controller.Login(Id, new LoginViewModel()) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.RouteValues.Any(v => v.Key == "action" && (string)v.Value == "Unlock"));
        }
    }
}
