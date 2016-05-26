using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenEnteringMyAccessCode
    {
        private const string EmployerPortalUrl = "http://employerportal.local";

        private Mock<AccountOrchestrator> _accountOrchestrator;
        private Mock<IConfigurationService> _configurationService;
        private AccountController _accountController;

        [SetUp]
        public void Arrange()
        {
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(Constants.ClaimTypes.Id, "myid"),
            })));
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(c => c.HttpContext).Returns(httpContext.Object);

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
            _accountController.ControllerContext = controllerContext.Object;
        }

        [Test]
        public void ThenWhenTheViewIsLoadedTheValidFlagIsTrue()
        {
            //Act
            var actual = _accountController.Confirm();

            //Assert
            Assert.IsNotNull(actual);
            var viewResult = actual as ViewResult;
            Assert.IsNotNull(viewResult);
            var actualModel = viewResult.Model as AccessCodeViewModel;
            Assert.IsNotNull(actualModel);
            Assert.IsTrue(actualModel.Valid);
        }

        [Test]
        public async Task ThenTheAccountOrchestratorAccessCodeIsCalled()
        {
            //Act
            await _accountController.Confirm(new AccessCodeViewModel());

            //Assert
            _accountOrchestrator.Verify(x=>x.ActivateUser(It.IsAny<AccessCodeViewModel>()),Times.Once);
        }

        [Test]
        public async Task ThenTheEmployerPortalIsReturnedWhenTheOrchestratorReturnsTrue()
        {
            //Arrange
            _accountOrchestrator.Setup(x => x.ActivateUser(It.IsAny<AccessCodeViewModel>())).ReturnsAsync(true);

            //Act
            var actual = await _accountController.Confirm(new AccessCodeViewModel());

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
            var actual = await _accountController.Confirm(new AccessCodeViewModel());

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
            await _accountController.Confirm(accessCodeViewModel);

            //Assert
            _accountOrchestrator.Verify(x => x.ActivateUser(It.Is<AccessCodeViewModel>(p=>p.AccessCode.Equals(accessCode) && p.UserId.Equals(userId))), Times.Once);
        }
    }
}
