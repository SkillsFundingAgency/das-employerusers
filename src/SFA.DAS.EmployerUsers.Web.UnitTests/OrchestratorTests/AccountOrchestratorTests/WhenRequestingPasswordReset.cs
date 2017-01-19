using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    [TestFixture]
    public class WhenRequestingPasswordReset
    {
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountOrchestrator _accountOrchestrator;
        private RequestPasswordResetViewModel _requestPasswordResetViewModel;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _logger = new Mock<ILogger>();

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);

            _requestPasswordResetViewModel = new RequestPasswordResetViewModel
            {
                Email = "test.user@test.org"
            };
        }

        [Test]
        public async Task ThenResetCodeIsSent()
        {
            var response = await _accountOrchestrator.RequestPasswordResetCode(_requestPasswordResetViewModel);

            Assert.That(response.Email, Is.EqualTo(_requestPasswordResetViewModel.Email));
            Assert.That(response.ResetCodeSent, Is.True);
            Assert.That(response.ErrorDictionary.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ThenResetCodeIsNotSentOnException()
        {
            var errors = new Dictionary<string, string>
            {
                { "Email", "Unknown email address" }
            };

            _mediator.Setup(x => x.SendAsync(It.Is<RequestPasswordResetCodeCommand>(m => m.Email == _requestPasswordResetViewModel.Email))).Throws(new InvalidRequestException(errors));

            var response = await _accountOrchestrator.RequestPasswordResetCode(_requestPasswordResetViewModel);

            Assert.That(response.Email, Is.EqualTo(_requestPasswordResetViewModel.Email));
            Assert.That(response.ResetCodeSent, Is.False);
            Assert.That(response.ErrorDictionary.Count, Is.EqualTo(1));
            Assert.That(response.ErrorDictionary.ContainsKey("Email"), Is.True);
        }

        [Test]
        public async Task ThenUserFriendlyErrorIsReturnedIfAccountIsNotFound()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<RequestPasswordResetCodeCommand>()))
                .ThrowsAsync(new UnknownAccountException());

            // Act
            var actual = await _accountOrchestrator.RequestPasswordResetCode(_requestPasswordResetViewModel);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
            Assert.AreEqual("Email address not registered", actual.EmailError);
        }
    }
}