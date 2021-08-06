using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenLoggingIn
    {
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountOrchestrator _orchestrator;
        private LoginViewModel _model;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<AuthenticateUserCommand>()))
                .Returns(Task.FromResult(new Domain.User { Email = "unit.tests@test.local", IsActive = true }));

            _owinWrapper = new Mock<IOwinWrapper>();

            _logger = new Mock<ILogger>();

            _orchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);

            _model = new LoginViewModel
            {
                EmailAddress = "unit.tests@test.local",
                Password = "password123"
            };
        }

        [Test]
        public async Task ThenItShouldReturnTrueWhenCommandReturnsUser()
        {
            // Act
            var actual = await _orchestrator.Login(_model);

            // Assert
            Assert.IsTrue(actual.Data.Success);
        }

        [Test]
        public async Task ThenItShouldReturnFalseWhenCommandDoesNotReturnUser()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<AuthenticateUserCommand>()))
                .Returns(Task.FromResult<Domain.User>(null));

            // Act
            var actual = await _orchestrator.Login(_model);

            // Assert
            Assert.IsFalse(actual.Data.Success);
        }

        [Test]
        public async Task ThenItShouldReturnFalseWhenCommandThrowsAnException()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<AuthenticateUserCommand>()))
                .ThrowsAsync(new Exception("Testing"));

            // Act
            var actual = await _orchestrator.Login(_model);

            // Assert
            Assert.IsFalse(actual.Data.Success);
        }

        [Test]
        public async Task ThenItShouldReturnRequiresActivationWhenUserIsNotActive()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<AuthenticateUserCommand>()))
                .Returns(Task.FromResult(new Domain.User { Email = "unit.tests@test.local", IsActive = false }));

            // Act
            var actual = await _orchestrator.Login(_model);

            // Assert
            Assert.IsTrue(actual.Data.RequiresActivation);
        }

        [Test]
        public async Task ThenItShouldNotReturnRequireActivationWhenUserIsActive()
        {
            // Act
            var actual = await _orchestrator.Login(_model);

            // Assert
            Assert.IsFalse(actual.Data.RequiresActivation);
        }

        [Test]
        public async Task ThenItShouldFalseWhenCommadnThrowsAccountLockedException()
        {
            //Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<AuthenticateUserCommand>()))
                .Throws(new AccountLockedException(new Domain.User { Email = "unit.tests@testing.local" }));

            // Act
            var actual = await _orchestrator.Login(_model);

            // Assert
            Assert.IsFalse(actual.Data.Success);
        }

        [Test]
        public async Task ThenItShouldAccountIsLockedWhenCommadnThrowsAccountLockedException()
        {
            //Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<AuthenticateUserCommand>()))
                .Throws(new AccountLockedException(new Domain.User { Email = "unit.tests@testing.local" }));

            // Act
            var actual = await _orchestrator.Login(_model);

            // Assert
            Assert.IsTrue(actual.Data.AccountIsLocked);
        }

        [Test]
        public async Task ThenAnInvalidRequestExceptionIsCaughtAndTheModelValidationMessagsAreSet()
        {
            //Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<AuthenticateUserCommand>()))
                .Throws(new InvalidRequestException(new Dictionary<string, string> { {"",""} }));

            //Act
            var actual = await _orchestrator.Login(_model);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest,actual.Status);
            Assert.AreEqual(1,actual.FlashMessage.ErrorMessages.Count);
        }
    }
}
