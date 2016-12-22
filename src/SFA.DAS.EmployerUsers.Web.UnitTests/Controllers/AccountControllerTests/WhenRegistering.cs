using System;
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
        private const string ClientId = "MyClient";
        private const string ReturnUrl = "https://localhost/identity/connect/authorize?p1=somestuff";

        private AccountController _accountController;
        private Mock<AccountOrchestrator> _accountOrchestator;
        private Mock<UrlHelper> _urlHelper;
        private Mock<ControllerContext> _controllerContext;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            ArrangeControllerContext("");

            _accountOrchestator = new Mock<AccountOrchestrator>();
            _accountOrchestator.Setup(o => o.StartRegistration(ClientId, ReturnUrl, true))
                .ReturnsAsync(new RegisterViewModel());

            _urlHelper = new Mock<UrlHelper>();
            _urlHelper.Setup(h => h.Action("Index", "Home", null, "https"))
                .Returns("https://localhost/");

            _accountController = new AccountController(_accountOrchestator.Object, null, null);
            _accountController.ControllerContext = _controllerContext.Object;
            _accountController.Url = _urlHelper.Object;
        }

        private void ArrangeControllerContext(string userId)
        {
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.Url)
                .Returns(new Uri("https://localhost"));

            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(DasClaimTypes.Id, userId),
            })));
            httpContext.Setup(c => c.Request)
                .Returns(request.Object);

            _controllerContext = new Mock<ControllerContext>();
            _controllerContext.Setup(c => c.HttpContext)
                .Returns(httpContext.Object);
        }

        [Test]
        public async Task ThenTheAccountOrchestratorRegisterIsCalled()
        {
            //Arrange
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterViewModel());

            //Act
            await _accountController.Register(new RegisterViewModel(), ReturnUrl);

            //Assert
            _accountOrchestator.Verify(x => x.Register(It.IsAny<RegisterViewModel>(), ReturnUrl));
        }

        [Test]
        public async Task ThenTheConfirmViewIsReturnedWhenTheOrchestratorReturnsTrue()
        {
            //Arrange
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterViewModel());

            //Act
            var actual = await _accountController.Register(new RegisterViewModel(), ReturnUrl);

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
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterViewModel { ErrorDictionary = new Dictionary<string, string> { { "Error", "Error" } } });

            //Act
            var actual = await _accountController.Register(new RegisterViewModel(), ReturnUrl);

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
        public async Task ThenIAmRedirectedToTheConfirmCodeIfITryToGoBackToRegisterWhenLoggedIn()
        {
            //Arrange
            ArrangeControllerContext("123456");
            _accountController.ControllerContext = _controllerContext.Object;
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterViewModel());

            //Act
            var actual = await _accountController.Register(ClientId, ReturnUrl);

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
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>(), It.IsAny<string>()))
                .ReturnsAsync(new RegisterViewModel());

            //Act
            var actual = await _accountController.Register(new RegisterViewModel(), ReturnUrl);

            //Assert
            _accountOrchestator.Verify(x => x.Register(It.IsAny<RegisterViewModel>(), It.IsAny<string>()), Times.Never);
            Assert.IsNotNull(actual);
            var redirectToRouteResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Confirm", redirectToRouteResult.RouteValues["Action"].ToString());

        }
    }
}
