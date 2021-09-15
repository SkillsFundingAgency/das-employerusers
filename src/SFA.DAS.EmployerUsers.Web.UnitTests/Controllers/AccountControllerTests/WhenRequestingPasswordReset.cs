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
using SFA.DAS.EmployerUsers.Web.Models.SFA.DAS.EAS.Web.Models;
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
        private OrchestratorResponse<RequestPasswordResetViewModel> _errorResponse;
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
            _errorResponse = new OrchestratorResponse<RequestPasswordResetViewModel>
            {
                Data = new RequestPasswordResetViewModel
                {
                    Email = _requestPasswordResetViewModel.Email,
                    ResetCodeSent = false,
                    ErrorDictionary = new Dictionary<string, string>
                    {
                        { "Email", "Error" }
                    }
                },
                FlashMessage = new FlashMessageViewModel
                {
                    ErrorMessages = new Dictionary<string, string>
                    {
                        { "Email", "Error" }
                    },
                    Headline = "There is a problem"
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
            // Arrange
            var response = new OrchestratorResponse<RequestPasswordResetViewModel>
            {
                Data = new RequestPasswordResetViewModel
                {
                    Email = _requestPasswordResetViewModel.Email,
                    ResetCodeSent = true
                }
            };

            _accountOrchestrator.Setup(x => x.RequestPasswordResetCode(It.Is<RequestPasswordResetViewModel>(m => m.Email == _requestPasswordResetViewModel.Email)))
                .ReturnsAsync(response);

            // Act
            var actual = await _accountController.ForgottenCredentials(_requestPasswordResetViewModel);

            // Assert
            var viewResult = (ViewResult) actual;
            var viewModel = (OrchestratorResponse<EnterResetCodeViewModel>) viewResult.Model;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(response.Data.Email,viewModel.Data.Email);
            Assert.AreEqual("EnterResetCode", viewResult.ViewName);
            
        }

        [Test]
        public async Task ThenTheResetCodeIsNotSentWhenAnErrorOccurs()
        {
            // Act
            var actionResult = await _accountController.ForgottenCredentials(_requestPasswordResetViewModel);

            var viewResult = (ViewResult)actionResult;
            var viewModel = (OrchestratorResponse<RequestPasswordResetViewModel>)viewResult.Model;

            // Assert
            Assert.That(viewResult.ViewName, Is.EqualTo("ForgottenCredentials"));
            Assert.That(viewModel.Data.Email, Is.EqualTo(_errorResponse.Data.Email));
            Assert.That(viewModel.Data.ResetCodeSent, Is.False);
            Assert.That(viewModel.Data.ErrorDictionary.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThenTheUserIsRedirectedToTheForgottenCredentialsPageIfTheEmailIsNotSupplied()
        {
            // Arrange
            _errorResponse.Data.Email = "";
            _accountOrchestrator.Setup(x => x.RequestPasswordResetCode(It.IsAny<RequestPasswordResetViewModel>())).ReturnsAsync(_errorResponse);

            // Act
            var actionResult = await _accountController.ForgottenCredentials(_requestPasswordResetViewModel);
            
            // Assert
            Assert.IsNotNull(actionResult);
            var viewResult = actionResult as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("ForgottenCredentials", viewResult.ViewName);
        }
        
    }
}