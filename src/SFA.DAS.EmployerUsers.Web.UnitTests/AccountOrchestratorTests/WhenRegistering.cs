using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators.Account;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.AccountOrchestratorTests
{
    public class WhenRegistering
    {
        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _accountOrchestrator = new AccountOrchestrator(_mediator.Object);
        }

        [Test]
        public void ThenTheConfirmationViewModelIsReturned()
        {
            //Arrange
            var registerUserViewModel = new RegisterViewModel();

            //Act
            var actual = _accountOrchestrator.Register(registerUserViewModel);

            //Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ThenTheRegisterUserCommandIsPassedOntoTheMediator()
        {
            //Arrange
            var registerUserViewModel = new RegisterViewModel();

            //Act
            _accountOrchestrator.Register(registerUserViewModel);

            //Assert
            _mediator.Verify(x=>x.Send(It.IsAny<RegisterUserCommand>()),Times.Once);
        }
    }
}
