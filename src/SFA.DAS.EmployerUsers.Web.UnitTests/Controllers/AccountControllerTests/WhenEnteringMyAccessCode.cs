using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenEnteringMyAccessCode
    {
        private AccountController _accountController;
        private Mock<AccountOrchestrator> _accountOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _accountOrchestrator = new Mock<AccountOrchestrator>();
            _accountController = new AccountController(_accountOrchestrator.Object);
        }

        [Test]
        public async Task ThenWhenTheViewIsLoadedTheValidFlagIsTrue()
        {
            //Act
            var actual = await _accountController.Confirm();

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
        public async Task ThenTheIndexOnHomeControllerIsReturnedWhenTheOrchestratorReturnsTrue()
        {
            //Arrange
            _accountOrchestrator.Setup(x => x.ActivateUser(It.IsAny<AccessCodeViewModel>())).ReturnsAsync(true);

            //Act
            var actual = await _accountController.Confirm(new AccessCodeViewModel());

            //Assert
            Assert.IsNotNull(actual);
            var redirectToRouteResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(redirectToRouteResult);
            Assert.AreEqual("Index", redirectToRouteResult.RouteValues["Action"].ToString());
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
            var userId = Guid.NewGuid().ToString();
            var accessCodeViewModel = new AccessCodeViewModel {AccessCode = accessCode, UserId = userId};
            await _accountController.Confirm(accessCodeViewModel);

            //Assert
            _accountOrchestrator.Verify(x => x.ActivateUser(It.Is<AccessCodeViewModel>(p=>p.AccessCode.Equals(accessCode) && p.UserId.Equals(userId))), Times.Once);
        }
    }
}
