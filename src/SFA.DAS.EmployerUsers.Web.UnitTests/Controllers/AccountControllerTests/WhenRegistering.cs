using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    public class WhenRegistering
    {
        private AccountController _accountController;
        private Mock<AccountOrchestrator> _accountOrchestator;

        [SetUp]
        public void Arrange()
        {
            _accountOrchestator = new Mock<AccountOrchestrator>();

            _accountController = new AccountController(_accountOrchestator.Object, null, null);
        }

        [Test]
        public async Task ThenTheAccountOrchestratorRegisterIsCalled()
        {
            //Arrange
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>())).ReturnsAsync(new RegisterResultModel());

            //Act
            await _accountController.Register(new RegisterViewModel());

            //Assert
            _accountOrchestator.Verify(x => x.Register(It.IsAny<RegisterViewModel>()));
        }

        [Test]
        public async Task ThenTheConfirmViewIsReturnedWhenTheOrchestratorReturnsTrue()
        {
            //Arrange
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>())).ReturnsAsync(new RegisterResultModel());

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
            _accountOrchestator.Setup(x => x.Register(It.IsAny<RegisterViewModel>())).ReturnsAsync(new RegisterResultModel { ErrorDictionary = new Dictionary<string, string> { { "Error", "Error" } } });

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
            Assert.IsFalse(actualModel.Valid);
            Assert.IsNotEmpty(actualModel.ErrorDictionary);
        }
    }
}
