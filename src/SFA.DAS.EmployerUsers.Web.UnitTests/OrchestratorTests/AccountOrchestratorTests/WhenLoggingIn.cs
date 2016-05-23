using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenLoggingIn
    {
        private AccountOrchestrator _orchestrator;
        private LoginViewModel _model;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<AuthenticateUserCommand>()))
                .Returns(Task.FromResult(new Domain.User {Email = "unit.tests@test.local" }));

            _orchestrator = new AccountOrchestrator(_mediator.Object);

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
            Assert.IsTrue(actual);
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
            Assert.IsFalse(actual);
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
            Assert.IsFalse(actual);
        }


    }
}
