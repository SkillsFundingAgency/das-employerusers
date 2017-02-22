using System.Threading.Tasks;
using System.Web.Mvc;

using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenChangingPassword : ControllerTestBase
    {
        private const string UserId = "USER1";
        private const string ClientId = "MyClient";
        private const string ReturnUrl = "http:?/unit.test";

        private Mock<AccountOrchestrator> _accountOrchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IConfigurationService> _configurationService;
        private AccountController _controller;
        private ChangePasswordViewModel _model;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            AddUserToContext(UserId);
            
            _accountOrchestrator = new Mock<AccountOrchestrator>();
            _accountOrchestrator.Setup(o => o.ChangePassword(It.IsAny<ChangePasswordViewModel>()))
                .Throws(new System.Exception("Called AccountOrchestrator.ChangePassword with incorrect model"));
            _accountOrchestrator.Setup(o => o.ChangePassword(It.Is<ChangePasswordViewModel>(m => m.UserId == UserId)))
                .Returns((ChangePasswordViewModel model) => Task.FromResult(new ChangePasswordViewModel
                {
                    UserId = model.CurrentPassword,
                    CurrentPassword = model.CurrentPassword,
                    NewPassword = model.NewPassword,
                    ConfirmPassword = model.ConfirmPassword
                }));

            _owinWrapper = new Mock<IOwinWrapper>();

            _configurationService = new Mock<IConfigurationService>();

            _controller = new AccountController(_accountOrchestrator.Object, _owinWrapper.Object, new IdentityServerConfiguration(), _logger.Object);
            _controller.ControllerContext = _controllerContext.Object;

            _model = new ChangePasswordViewModel
            {
                CurrentPassword = "password",
                NewPassword = "newpassword",
                ConfirmPassword = "newpassword"
            };
        }

        [Test]
        public async Task ThenItShouldRedirectToReturnUrlIfNoErrors()
        {
            // Act
            var actual = await _controller.ChangePassword(_model, ClientId, ReturnUrl);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<RedirectResult>(actual);
            Assert.AreEqual(ReturnUrl, ((RedirectResult)actual).Url);
        }

        [Test]
        public async Task ThenItShouldReturnViewIfRequestNotValid()
        {
            // Arrange
            _accountOrchestrator.Setup(o => o.ChangePassword(It.Is<ChangePasswordViewModel>(m => m.UserId == UserId)))
                   .Returns((ChangePasswordViewModel model) => Task.FromResult(new ChangePasswordViewModel
                   {
                       UserId = model.CurrentPassword,
                       CurrentPassword = model.CurrentPassword,
                       NewPassword = model.NewPassword,
                       ConfirmPassword = model.ConfirmPassword,
                       ErrorDictionary = new System.Collections.Generic.Dictionary<string, string> { { "", "Error" } }
                   }));

            // Act
            var actual = await _controller.ChangePassword(_model, ClientId, ReturnUrl);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOf<ViewResult>(actual);
        }

        [Test]
        public async Task ThenItShouldReturnModelWithPasswordsBlankedIfRequestNotValid()
        {
            // Arrange
            _accountOrchestrator.Setup(o => o.ChangePassword(It.Is<ChangePasswordViewModel>(m => m.UserId == UserId)))
                   .Returns((ChangePasswordViewModel model) => Task.FromResult(new ChangePasswordViewModel
                   {
                       UserId = model.CurrentPassword,
                       CurrentPassword = model.CurrentPassword,
                       NewPassword = model.NewPassword,
                       ConfirmPassword = model.ConfirmPassword,
                       ErrorDictionary = new System.Collections.Generic.Dictionary<string, string> { { "", "Error" } }
                   }));

            // Act
            var actual = (ViewResult)await _controller.ChangePassword(_model, ClientId, ReturnUrl);

            // Assert
            Assert.IsNotNull(actual.Model);
            Assert.IsInstanceOf<OrchestratorResponse<ChangePasswordViewModel>>(actual.Model);

            var actualModel = (OrchestratorResponse<ChangePasswordViewModel>)actual.Model;
            Assert.IsEmpty(actualModel.Data.CurrentPassword);
            Assert.IsEmpty(actualModel.Data.NewPassword);
            Assert.IsEmpty(actualModel.Data.ConfirmPassword);
        }
    }
}
