using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
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
    public class WhenConfirmingChangeEmail : ControllerTestBase
    {
        private const string UserId = "USER1";
        private const string SecurityCode = "1A2B3C";
        private const string Password = "Pa55word1";
        private const string ReturnUrl = "http://relying.party";

        private Mock<AccountOrchestrator> _accountOrchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountController _controller;
        private ConfirmChangeEmailViewModel _model;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            AddUserToContext(UserId);

            _accountOrchestrator = new Mock<AccountOrchestrator>();
            _accountOrchestrator.Setup(o => o.ConfirmChangeEmail(It.IsAny<ConfirmChangeEmailViewModel>()))
                .Returns((ConfirmChangeEmailViewModel model) => Task.FromResult(new ConfirmChangeEmailViewModel
                {
                    SecurityCode = model.SecurityCode,
                    Password = model.Password,
                    UserId = model.UserId,
                    ReturnUrl = ReturnUrl
                }));

            _owinWrapper = new Mock<IOwinWrapper>();

            _controller = new AccountController(_accountOrchestrator.Object, _owinWrapper.Object, new IdentityServerConfiguration());
            _controller.ControllerContext = _controllerContext.Object;

            _model = new ConfirmChangeEmailViewModel
            {
                SecurityCode = SecurityCode,
                Password = Password
            };
        }

        [Test]
        public async Task ThenItShouldAttachTheUserIdAndCallTheOrchestrator()
        {
            // Act
            await _controller.ConfirmChangeEmail(_model);

            // Assert
            _accountOrchestrator.Verify(o => o.ConfirmChangeEmail(It.Is<ConfirmChangeEmailViewModel>(m => m.UserId == UserId)),
                                        Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnTheViewIfTheResponseIsNotValid()
        {
            // Arrange
            _accountOrchestrator.Setup(o => o.ConfirmChangeEmail(It.IsAny<ConfirmChangeEmailViewModel>()))
                .Returns(Task.FromResult(new ConfirmChangeEmailViewModel
                {
                    SecurityCode = SecurityCode,
                    Password = Password,
                    ErrorDictionary = new Dictionary<string, string>
                    {
                        {"", "Error"}
                    }
                }));

            // Act
            var actual = await _controller.ConfirmChangeEmail(_model);

            // Asssert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
        }

        [Test]
        public async Task ThenItShouldClearTheSecurityCodeIfTheResponseIsNotValid()
        {
            // Arrange
            _accountOrchestrator.Setup(o => o.ConfirmChangeEmail(It.IsAny<ConfirmChangeEmailViewModel>()))
                .Returns(Task.FromResult(new ConfirmChangeEmailViewModel
                {
                    SecurityCode = SecurityCode,
                    Password = Password,
                    ErrorDictionary = new Dictionary<string, string>
                    {
                        {"", "Error"}
                    }
                }));

            // Act
            var actual = await _controller.ConfirmChangeEmail(_model);

            // Asssert
            var actualModel = ((ViewResult)actual).Model as OrchestratorResponse<ConfirmChangeEmailViewModel>;
            Assert.IsNotNull(actualModel);
            Assert.IsEmpty(actualModel.Data.SecurityCode);
        }

        [Test]
        public async Task ThenItShouldClearThePasswordIfTheResponseIsNotValid()
        {
            // Arrange
            _accountOrchestrator.Setup(o => o.ConfirmChangeEmail(It.IsAny<ConfirmChangeEmailViewModel>()))
                .Returns(Task.FromResult(new ConfirmChangeEmailViewModel
                {
                    SecurityCode = SecurityCode,
                    Password = Password,
                    ErrorDictionary = new Dictionary<string, string>
                    {
                        {"", "Error"}
                    }
                }));

            // Act
            var actual = await _controller.ConfirmChangeEmail(_model);

            // Asssert
            var actualModel = ((ViewResult)actual).Model as OrchestratorResponse<ConfirmChangeEmailViewModel>;
            Assert.IsNotNull(actualModel);
            Assert.IsEmpty(actualModel.Data.Password);
        }

        [Test]
        public async Task ThenItShouldRedirectBackToReturnUrlIfResponseIsValid()
        {
            // Act
            var actual = await _controller.ConfirmChangeEmail(_model);

            // Asssert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<RedirectResult>(actual);
            Assert.AreEqual(ReturnUrl, ((RedirectResult)actual).Url);
        }
    }
}
