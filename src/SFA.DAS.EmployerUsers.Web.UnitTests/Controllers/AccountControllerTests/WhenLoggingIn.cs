using System.Threading.Tasks;
using System.Web.Mvc;
using IdentityServer3.Core.Models;
using Microsoft.Owin;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenLoggingIn
    {
        private const string Id = "UNIT_TESTS";
        private const string ReturnUrl = "http://unittests.local";

        private Mock<AccountOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountController _controller;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<AccountOrchestrator>();

            _owinWrapper = new Mock<IOwinWrapper>();
            _owinWrapper.Setup(w => w.GetSignInMessage(Id))
                .Returns(new SignInMessage
                {
                    ReturnUrl = ReturnUrl
                });

            _controller = new AccountController(_orchestrator.Object, _owinWrapper.Object);
        }

        [Test]
        public async Task ThenItShouldReturnViewIfUnsuccessful()
        {
            // Act
            var actual = await _controller.Login(Id, new Models.LoginViewModel());

            // Assert
            Assert.IsInstanceOf<ViewResult>(actual);
        }

        [Test]
        public async Task ThenItShouldReturnTrueAsModelIfUnsuccessful()
        {
            // Act
            var actual = (ViewResult)await _controller.Login(Id, new Models.LoginViewModel());

            // Assert
            Assert.IsTrue((bool)actual.Model);
        }

        [Test]
        public async Task ThenItShouldReturnARedirectIfSuccessful()
        {
            // Arrange
            _orchestrator.Setup(o => o.Login(It.IsAny<Models.LoginViewModel>())).Returns(Task.FromResult(true));

            // Act
            var actual = await _controller.Login(Id, new Models.LoginViewModel());

            // Assert
            Assert.IsInstanceOf<RedirectResult>(actual);
            Assert.AreEqual(ReturnUrl, ((RedirectResult) actual).Url);
        }
    }
}
