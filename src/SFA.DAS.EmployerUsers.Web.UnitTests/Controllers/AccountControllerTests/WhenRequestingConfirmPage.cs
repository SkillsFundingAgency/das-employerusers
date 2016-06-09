using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenRequestingConfirmPage : ControllerTestBase
    {
        private Mock<AccountOrchestrator> _accountOrchestrator;
        private AccountController _controller;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            AddUserToContext();

            _accountOrchestrator = new Mock<AccountOrchestrator>();
            _accountOrchestrator.Setup(o => o.RequestConfirmAccount(It.IsAny<string>())).Returns(Task.FromResult(true));

            _controller = new AccountController(_accountOrchestrator.Object, null, null);
            _controller.ControllerContext = _controllerContext.Object;
        }

        [Test]
        public async Task ThenItShouldReturnAConfirmViewResultIfAccountIsNotActive()
        {
            // Act
            var actual = await _controller.Confirm();

            // Assert
            Assert.IsInstanceOf<ViewResult>(actual);
            Assert.AreEqual("Confirm", ((ViewResult) actual).ViewName);
        }

        [Test]
        public async Task ThenItShouldReturnARedirectToHomePageIfAccountIsActive()
        {
            // Arrange
            _accountOrchestrator.Setup(o => o.RequestConfirmAccount(It.IsAny<string>())).Returns(Task.FromResult(false));

            // Act
            var actual = await _controller.Confirm();

            // Assert
            Assert.IsInstanceOf<RedirectToRouteResult>(actual);
            Assert.AreEqual("Home", ((RedirectToRouteResult)actual).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)actual).RouteValues["action"]);
        }
    }
}
