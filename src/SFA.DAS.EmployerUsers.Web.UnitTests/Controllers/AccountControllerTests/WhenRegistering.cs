using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenRegistering : ControllerTestBase
    {
        private AccountController _accountController;
        private Mock<AccountOrchestrator> _accountOrchestator;
        private Mock<ControllerContext> _controllerContext;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            ArrangeControllerContext("");

            _accountOrchestator = new Mock<AccountOrchestrator>();

            _accountController = new AccountController(_accountOrchestator.Object, null, null);
            _accountController.ControllerContext = _controllerContext.Object;
        }

        private void ArrangeControllerContext(string userId)
        {
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, userId),
            })));
            _controllerContext = new Mock<ControllerContext>();
            _controllerContext.Setup(c => c.HttpContext).Returns(httpContext.Object);
        }

        [Test]
        public async Task ThenTheAccountOrchestratorRegisterIsCalled()
        {
            //Arrange
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>())).ReturnsAsync(new RegisterViewModel());

            //Act
            await _accountController.Register(new RegisterViewModel());

            //Assert
            _accountOrchestator.Verify(x => x.Register(It.IsAny<RegisterViewModel>()));
        }

        [Test]
        public async Task ThenTheConfirmViewIsReturnedWhenTheOrchestratorReturnsTrue()
        {
            //Arrange
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>())).ReturnsAsync(new RegisterViewModel());

            //Act
            var actual = await _accountController.Register(new RegisterViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var redirectToRouteResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Confirm", redirectToRouteResult.RouteValues["Action"].ToString());

        }

        [Test]
        public async Task ThenTheRegisterViewIsReturnedWhenTheOrchestratorReturnsFalse()
        {
            //Arrange
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>())).ReturnsAsync(new RegisterViewModel { ErrorDictionary = new Dictionary<string, string> { { "Error", "Error" } } });

            //Act
            var actual = await _accountController.Register(new RegisterViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual("Register", actualViewResult.ViewName);
            Assert.IsAssignableFrom<RegisterViewModel>(actualViewResult.Model);
            var actualModel = actualViewResult.Model as RegisterViewModel;
            Assert.IsNotNull(actualModel);
        }

        [Test]
        public void ThenIAmRedirectedToTheConfirmCodeIfITryToGoBackToRegisterWhenLoggedIn()
        {
            //Arrange
            ArrangeControllerContext("123456");
            _accountController.ControllerContext = _controllerContext.Object;
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>())).ReturnsAsync(new RegisterViewModel());

            //Act
            var actual = _accountController.Register();

            //Assert
            Assert.IsNotNull(actual);
            var redirectToRouteResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Confirm", redirectToRouteResult.RouteValues["Action"].ToString());

        }

        [Test]
        public async Task ThenIAmRedirectedToTheConfirmCodeIfITryToReSubmitMyRegistrationWhenLoggedIn()
        {
            //Arrange
            ArrangeControllerContext("123456");
            _accountController.ControllerContext = _controllerContext.Object;
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>())).ReturnsAsync(new RegisterViewModel());

            //Act
            var actual = await _accountController.Register(new RegisterViewModel());

            //Assert
            _accountOrchestator.Verify(x=>x.Register(It.IsAny<RegisterViewModel>()),Times.Never);
            Assert.IsNotNull(actual);
            var redirectToRouteResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Confirm", redirectToRouteResult.RouteValues["Action"].ToString());

        }
    }
}
