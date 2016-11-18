using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
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
                .ReturnsAsync(new ActivateUserViewModel { Valid = true, ReturnUrl = ReturnUrl });

            _configurationService = new Mock<IConfigurationService>();

            _accountController = new AccountController(_accountOrchestrator.Object, null, _configurationService.Object);
            _accountController.ControllerContext = _controllerContext.Object;
        }


        [Test]
        public async Task ThenTheAccountOrchestratorAccessCodeIsCalled()
        {
            //Act
            await _accountController.Confirm(new ActivateUserViewModel(), Action);

            //Assert
            _accountOrchestrator.Verify(x=>x.ActivateUser(It.IsAny<ActivateUserViewModel>()),Times.Once);
        }

        [Test]
        public async Task ThenRedirectsToOriginWhenActivationSuccessful()
        {
            //Act
            var actual = await _accountController.Confirm(new ActivateUserViewModel(), Action);

            //Assert
            Assert.IsNotNull(actual);
            var redirectResult = actual as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(ReturnUrl, redirectResult.Url);
        }

        [Test]
        public async Task ThenTheConfirmViewIsReturnedWhenTheOrchestratorReturnsFalse()
        {
            //Arrange
            _accountOrchestrator.Setup(x => x.ActivateUser(It.IsAny<ActivateUserViewModel>()))
                .ReturnsAsync(new ActivateUserViewModel { Valid = false });

            //Act
            var actual = await _accountController.Confirm(new ActivateUserViewModel(), Action);

            //Assert
            Assert.IsNotNull(actual);
            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Confirm", viewResult.ViewName);
            var model = viewResult.Model as ActivateUserViewModel;
            Assert.IsNotNull(model);
            Assert.IsFalse(model.Valid);
        }

        [Test]
        public async Task ThenTheViewModelValuesArePassedToTheOrchestrator()
        {
            //Act
            var accessCode = "myCode";
            var userId = "myid";
            var accessCodeViewModel = new ActivateUserViewModel {AccessCode = accessCode, UserId = userId};
            await _accountController.Confirm(accessCodeViewModel, Action);

            //Assert
            _accountOrchestrator.Verify(x => x.ActivateUser(It.Is<ActivateUserViewModel>(p=>p.AccessCode.Equals(accessCode) && p.UserId.Equals(userId))), Times.Once);
        }
    }
}
