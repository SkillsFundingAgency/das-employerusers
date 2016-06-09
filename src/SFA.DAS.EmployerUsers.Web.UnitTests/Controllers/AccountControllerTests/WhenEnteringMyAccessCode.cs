using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityServer3.Core;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenEnteringMyAccessCode : ControllerTestBase
    {
        private const string EmployerPortalUrl = "http://employerportal.local";
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

            _configurationService = new Mock<IConfigurationService>();
            _configurationService.Setup(s => s.GetAsync<EmployerUsersConfiguration>())
                .Returns(Task.FromResult(new EmployerUsersConfiguration
                {
                    IdentityServer = new IdentityServerConfiguration
                    {
                        EmployerPortalUrl = EmployerPortalUrl
                    }
                }));

            _accountController = new AccountController(_accountOrchestrator.Object, null, _configurationService.Object);
            _accountController.ControllerContext = _controllerContext.Object;
        }


        [Test]
        public async Task ThenTheAccountOrchestratorAccessCodeIsCalled()
        {
            //Act
            await _accountController.Confirm(new AccessCodeViewModel(), Action);

            //Assert
            _accountOrchestrator.Verify(x=>x.ActivateUser(It.IsAny<AccessCodeViewModel>()),Times.Once);
        }

        [Test]
        public async Task ThenTheEmployerPortalIsReturnedWhenTheOrchestratorReturnsTrue()
        {
            //Arrange
            _accountOrchestrator.Setup(x => x.ActivateUser(It.IsAny<AccessCodeViewModel>())).ReturnsAsync(true);

            //Act
            var actual = await _accountController.Confirm(new AccessCodeViewModel(), Action);

            //Assert
            Assert.IsNotNull(actual);
            var redirectResult = actual as RedirectResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(EmployerPortalUrl, redirectResult.Url);
        }

        [Test]
        public async Task ThenTheConfirmViewIsReturnedWhenTheOrchestratorReturnsFalse()
        {
            //Arrange
            _accountOrchestrator.Setup(x => x.ActivateUser(It.IsAny<AccessCodeViewModel>())).ReturnsAsync(false);

            //Act
            var actual = await _accountController.Confirm(new AccessCodeViewModel(), Action);

            //Assert
            Assert.IsNotNull(actual);
            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("Confirm", viewResult.ViewName);
            var model = viewResult.Model as AccessCodeViewModel;
            Assert.IsNotNull(model);
            Assert.IsFalse(model.Valid);
        }

        [Test]
        public async Task ThenTheViewModelValuesArePassedToTheOrchestrator()
        {
            //Act
            var accessCode = "myCode";
            var userId = "myid";
            var accessCodeViewModel = new AccessCodeViewModel {AccessCode = accessCode, UserId = userId};
            await _accountController.Confirm(accessCodeViewModel, Action);

            //Assert
            _accountOrchestrator.Verify(x => x.ActivateUser(It.Is<AccessCodeViewModel>(p=>p.AccessCode.Equals(accessCode) && p.UserId.Equals(userId))), Times.Once);
        }
    }
}
