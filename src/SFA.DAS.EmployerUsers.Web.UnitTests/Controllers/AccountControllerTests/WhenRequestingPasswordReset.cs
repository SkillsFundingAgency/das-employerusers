using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    [TestFixture]
    public class WhenRequestingPasswordReset 
    {
        private Mock<AccountOrchestrator> _accountOrchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IConfigurationService> _configurationService;
        private AccountController _accountController;
        private RequestPasswordResetViewModel _requestPasswordResetViewModel;
        private RequestPasswordResetViewModel _errorResponse;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
            _requestPasswordResetViewModel = new RequestPasswordResetViewModel
            {
                Email = "test.user@test.org"
            };

            _accountOrchestrator = new Mock<AccountOrchestrator>();
            _errorResponse = new RequestPasswordResetViewModel
            {
                Email = _requestPasswordResetViewModel.Email,
                ResetCodeSent = false,
                ErrorDictionary = new Dictionary<string, string>
                {
                    { "Email", "Error" }
                }
            };
            _accountOrchestrator.Setup(x => x.RequestPasswordResetCode(It.Is<RequestPasswordResetViewModel>(m => m.Email == _requestPasswordResetViewModel.Email))).ReturnsAsync(_errorResponse);

            _owinWrapper = new Mock<IOwinWrapper>();
            _configurationService = new Mock<IConfigurationService>();
            _accountController = new AccountController(_accountOrchestrator.Object, _owinWrapper.Object, new IdentityServerConfiguration(), _logger.Object);

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

            var actual = await _accountController.ForgottenCredentials(_requestPasswordResetViewModel, "");

            var viewResult = (ViewResult) actual;
            var viewModel = (OrchestratorResponse<PasswordResetViewModel>) viewResult.Model;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(response.Email,viewModel.Data.Email);
            Assert.AreEqual("ResetPassword", viewResult.ViewName);
            
        }

        [Test]
        public async Task ThenTheResetCodeIsNotSentWhenAnErrorOccurs()
        {
           
            var xyz = await _accountController.ForgottenCredentials(_requestPasswordResetViewModel, "");

            var viewResult = (ViewResult)xyz;
            var viewModel = (RequestPasswordResetViewModel)viewResult.Model;

            Assert.That(viewResult.ViewName, Is.EqualTo("ForgottenCredentials"));
            Assert.That(viewModel.Email, Is.EqualTo(_errorResponse.Email));
            Assert.That(viewModel.ResetCodeSent, Is.False);
            Assert.That(viewModel.ErrorDictionary.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThenTheUserIsRedirectedToTheForgottenCredentialsPageIfTheEmailIsNotSupplied()
        {
            //Arrange
            _errorResponse.Email = "";
            _accountOrchestrator.Setup(x => x.RequestPasswordResetCode(It.IsAny<RequestPasswordResetViewModel>())).ReturnsAsync(_errorResponse);

            //Act
            var actual = await _accountController.ForgottenCredentials(_requestPasswordResetViewModel, "");

            Assert.IsNotNull(actual);
            var actualViewResult = actual as ViewResult;
            Assert.IsNotNull(actualViewResult);
            Assert.AreEqual("ForgottenCredentials",actualViewResult.ViewName);
        }
        
    }
}