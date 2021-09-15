using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenEnteringMyAccessCode : ControllerTestBase
    {
        private const string ReturnUrl = "http://unit.test";
        private const string Action = "activate";

        private Mock<AccountOrchestrator> _accountOrchestrator;
        private Mock<IConfigurationService> _configurationService;
        private AccountController _accountController;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();

            AddUserToContext("myid");

            _accountOrchestrator = new Mock<AccountOrchestrator>();
            _accountOrchestrator.Setup(x => x.ActivateUser(It.IsAny<ActivateUserViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<ActivateUserViewModel> { Data = new ActivateUserViewModel { ReturnUrl = ReturnUrl } });

            _configurationService = new Mock<IConfigurationService>();

            _accountController = new AccountController(_accountOrchestrator.Object, null, new IdentityServerConfiguration(), _logger.Object);
            _accountController.ControllerContext = _controllerContext.Object;
        }


        [Test]
        public async Task ThenTheAccountOrchestratorAccessCodeIsCalled()
        {
            // Act
            await _accountController.Confirm(new ActivateUserViewModel());

            // Assert
            _accountOrchestrator.Verify(x=>x.ActivateUser(It.IsAny<ActivateUserViewModel>()),Times.Once);
        }

        [Test]
        public async Task ThenRedirectsToOriginWhenActivationSuccessful()
        {
            // Act
            var actual = await _accountController.Confirm(new ActivateUserViewModel());

            // Assert
            Assert.IsNotNull(actual);
            var redirectResult = actual as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(ReturnUrl, redirectResult.Url);
        }

        [Test]
        public async Task ThenTheConfirmViewIsReturnedWhenTheOrchestratorReturnsFalse()
        {
            // Arrange
            _accountOrchestrator.Setup(x => x.ActivateUser(It.IsAny<ActivateUserViewModel>()))
                .ReturnsAsync(new OrchestratorResponse<ActivateUserViewModel>
                {
                    Data = new ActivateUserViewModel
                    {
                        ErrorDictionary = new Dictionary<string, string> { { "Error", "Error Message"}}
                    }
                });

            // Act
            var actual = await _accountController.Confirm(new ActivateUserViewModel());

            // Assert
            Assert.IsNotNull(actual);
            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Confirm", viewResult.ViewName);
            var model = viewResult.Model as OrchestratorResponse<ActivateUserViewModel>;
            Assert.IsNotNull(model);
            Assert.IsFalse(model.Data.Valid);
        }

        [Test]
        public async Task ThenTheViewModelValuesArePassedToTheOrchestrator()
        {
            // Act
            var accessCode = "myCode";
            var userId = "myid";
            var accessCodeViewModel = new ActivateUserViewModel {AccessCode = accessCode, UserId = userId};
            await _accountController.Confirm(accessCodeViewModel);

            // Assert
            _accountOrchestrator.Verify(x => x.ActivateUser(It.Is<ActivateUserViewModel>(p=>p.AccessCode.Equals(accessCode) && p.UserId.Equals(userId))), Times.Once);
        }
    }
}
