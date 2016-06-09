using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Commands.UnlockUser;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenUnlockingUser
    {
        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object);
        }

        [Test]
        public async Task ThenABooleanIsReturned()
        {
            //Arrange
            var unlockUserViewModel = new UnlockUserViewModel();

            //act
            var actual = await _accountOrchestrator.UnlockUser(unlockUserViewModel);

            //Assert
            Assert.IsAssignableFrom<UnlockUserViewModel>(actual);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenTheUnlockUserCommandIsPassedToTheMediator()
        {
            //Arrange
            var unlockCode = "123EWQ321";
            var email = "email@local";
            var unlockUserViewModel = new UnlockUserViewModel { UnlockCode = unlockCode, Email = email };

            //Act
            var actual = await _accountOrchestrator.UnlockUser(unlockUserViewModel);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<UnlockUserCommand>(p => p.Email.Equals(email) && p.UnlockCode.Equals(unlockCode))), Times.Once);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenFalseIsReturnedIfAnArgumentNullExceptionIsThrown()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<UnlockUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _accountOrchestrator.UnlockUser(new UnlockUserViewModel());

            //Assert
            Assert.IsFalse(actual.Valid);

        }


        [Test]
        public async Task ThenTheActivateUserCommandIsNotCaleldWhenAnArgumentNullExceptionIsThrown()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<UnlockUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            await _accountOrchestrator.UnlockUser(new UnlockUserViewModel());

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<ActivateUserCommand>()), Times.Never);

        }

        [Test]
        public async Task ThenTheActivateUserCommandIsPassedToTheMediatorForAValidUnlockUserCommand()
        {
            //Arrange
            var unlockCode = "123EWQ321";
            var email = "email@local";
            var unlockUserViewModel = new UnlockUserViewModel { UnlockCode = unlockCode, Email = email };

            //Act
            var actual = await _accountOrchestrator.UnlockUser(unlockUserViewModel);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<ActivateUserCommand>(p=>p.Email == email)),Times.Once);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenTheUnlockCodeExpiredFlagIsSetToTrueIfTheRequestIsInvalidAndTheErrorDictionaryContainsTheKey()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<UnlockUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "UnlockCodeExpiry", "Unlock code expired"} }));

            //Act
            var actual = await _accountOrchestrator.UnlockUser(new UnlockUserViewModel());

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<ActivateUserCommand>()), Times.Never);
            Assert.IsTrue(actual.UnlockCodeExpiry);
        }
    }
}
