﻿using System.Threading.Tasks;
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
    [TestFixture]
    public class WhenResendingActivationCode : ControllerTestBase
    {
        private const string EmployerPortalUrl = "http://employerportal.local";
        private const string Action = "resend";
        private const string UserId = "myid";

        private Mock<AccountOrchestrator> _accountOrchestrator;
        private Mock<IConfigurationService> _configurationService;
        private AccountController _accountController;

        [SetUp]
        public override void Arrange()
        {
            base.Arrange();
            AddUserToContext(UserId);

            _accountOrchestrator = new Mock<AccountOrchestrator>();

            _configurationService = new Mock<IConfigurationService>();

            _accountController = new AccountController(_accountOrchestrator.Object, null, new IdentityServerConfiguration(), _logger.Object)
            {
                ControllerContext = _controllerContext.Object
            };
        }

        [Test]
        public async Task ThenTheViewModelValuesArePassedToTheOrchestrator()
        {
            var accessCodeViewModel = new ActivateUserViewModel();

            var actual = await _accountController.ConfirmResend();

            _accountOrchestrator.Verify(x => x.ResendActivationCode(It.Is<ResendActivationCodeViewModel>(p => p.UserId.Equals(UserId))), Times.Once);

            var viewResult = actual as RedirectToRouteResult;
            Assert.IsNotNull(viewResult);
            Assert.That(viewResult.RouteValues["Action"], Is.EqualTo("Confirm"));
        }

        //TODO: Create negative test case for this.
    }
}