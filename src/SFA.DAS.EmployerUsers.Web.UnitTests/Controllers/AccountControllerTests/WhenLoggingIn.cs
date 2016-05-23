using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenLoggingIn
    {
        private Mock<AccountOrchestrator> _orchestrator;
        private AccountController _controller;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<AccountOrchestrator>();

            _controller = new AccountController(_orchestrator.Object);
        }

        [Test]
        public async Task ThenItShouldReturnViewIfUnsuccessful()
        {
            // Act
            var actual = await _controller.Login(new Models.LoginViewModel());

            // Assert
            Assert.IsInstanceOf<ViewResult>(actual);
        }

        [Test]
        public async Task ThenItShouldReturnTrueAsModelIfUnsuccessful()
        {
            // Act
            var actual = (ViewResult)await _controller.Login(new Models.LoginViewModel());

            // Assert
            Assert.IsTrue((bool)actual.Model);
        }

        [Test]
        public async Task ThenItShouldReturnARedirectIfSuccessful()
        {
            // Arrange
            _orchestrator.Setup(o => o.Login(It.IsAny<Models.LoginViewModel>())).Returns(Task.FromResult(true));

            // Act
            var actual = await _controller.Login(new Models.LoginViewModel());

            // Assert
            Assert.IsInstanceOf<RedirectResult>(actual);
        }
    }
}
