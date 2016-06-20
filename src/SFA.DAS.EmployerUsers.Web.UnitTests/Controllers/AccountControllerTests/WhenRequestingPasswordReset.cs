using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Controllers;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.Controllers.AccountControllerTests
{
    [TestFixture]
    public class WhenRequestingPasswordReset
    {
        private Mock<AccountOrchestrator> _accountOrchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IConfigurationService> _configurationService;
        private AccountController _accountController;
        private RequestPasswordResetViewModel _requestPasswordResetViewModel;

        [SetUp]
        public void Setup()
        {
            _accountOrchestrator = new Mock<AccountOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _configurationService = new Mock<IConfigurationService>();
            _accountController = new AccountController(_accountOrchestrator.Object, _owinWrapper.Object, _configurationService.Object);

            _requestPasswordResetViewModel = new RequestPasswordResetViewModel
            {
                Email = "test.user@test.org"
            };
        }

        [Test]
        public async Task ThenTheResetCodeIsSent()
        {
            var response = new RequestPasswordResetViewModel
            {
                Email = _requestPasswordResetViewModel.Email,
                ResetCodeSent = true
            };

            _accountOrchestrator.Setup(x => x.RequestPasswordResetCode(It.Is<RequestPasswordResetViewModel>(m => m.Email == _requestPasswordResetViewModel.Email)))
                .ReturnsAsync(response);

            var xyz = await _accountController.ForgottenCredentials(_requestPasswordResetViewModel);

            var viewResult = (ViewResult) xyz;
            var viewModel = (RequestPasswordResetViewModel) viewResult.Model;

            Assert.That(viewResult.ViewName, Is.EqualTo("ForgottenCredentials"));
            Assert.That(viewModel.Email, Is.EqualTo(response.Email));
            Assert.That(viewModel.ResetCodeSent, Is.True);
            Assert.That(viewModel.ErrorDictionary.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ThenTheResetCodeIsNotSentWhenAnErrorOccurs()
        {
            var response = new RequestPasswordResetViewModel
            {
                Email = _requestPasswordResetViewModel.Email,
                ResetCodeSent = false,
                ErrorDictionary = new Dictionary<string, string>
                {
                    { "Email", "Error" }
                }
            };

            _accountOrchestrator.Setup(x => x.RequestPasswordResetCode(It.Is<RequestPasswordResetViewModel>(m => m.Email == _requestPasswordResetViewModel.Email)))
                .ReturnsAsync(response);

            var xyz = await _accountController.ForgottenCredentials(_requestPasswordResetViewModel);

            var viewResult = (ViewResult)xyz;
            var viewModel = (RequestPasswordResetViewModel)viewResult.Model;

            Assert.That(viewResult.ViewName, Is.EqualTo("ForgottenCredentials"));
            Assert.That(viewModel.Email, Is.EqualTo(response.Email));
            Assert.That(viewModel.ResetCodeSent, Is.False);
            Assert.That(viewModel.ErrorDictionary.Count, Is.EqualTo(1));
        }
    }
}