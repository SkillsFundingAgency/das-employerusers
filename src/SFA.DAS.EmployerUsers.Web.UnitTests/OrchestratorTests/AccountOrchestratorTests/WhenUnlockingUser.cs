using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Commands.UnlockUser;
using SFA.DAS.EmployerUsers.Application.Exceptions;
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
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _logger = new Mock<ILogger>();

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);
        }

        [Test]
        public async Task ThenAUnlockUserViewModelOrchestratorResponseIsReturned()
        {
            //Arrange
            var unlockUserViewModel = new UnlockUserViewModel();

            //act
            var actual = await _accountOrchestrator.UnlockUser(unlockUserViewModel);

            //Assert
            Assert.IsAssignableFrom<OrchestratorResponse<UnlockUserViewModel>>(actual);
            Assert.IsTrue(actual.Data.Valid);
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
            Assert.IsTrue(actual.Data.Valid);
        }

        [Test]
        public async Task ThenFalseIsReturnedIfAnArgumentNullExceptionIsThrown()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<UnlockUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { {"",""} }));

            //Act
            var actual = await _accountOrchestrator.UnlockUser(new UnlockUserViewModel());

            //Assert
            Assert.IsFalse(actual.Data.Valid);

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
            Assert.IsTrue(actual.Data.Valid);
        }

        [Test]
        public async Task ThenTheUnlockCodeExpiredFlagIsSetToTrueIfTheRequestIsInvalidAndTheErrorDictionaryContainsTheKey()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<UnlockUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "UnlockCodeExpired", "Unlock code expired"} }));

            //Act
            var actual = await _accountOrchestrator.UnlockUser(new UnlockUserViewModel());

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<ActivateUserCommand>()), Times.Never);
            Assert.IsTrue(actual.Data.UnlockCodeExpired);
        }

        [Test]
        public async Task ThenTheErrorsAreCorrectlyMappedWhenAllFieldsHaveFailedValidation()
        {
            //Arrange
            var unlockCodeError = "unlock code Error";
            var emailError = "Email Error";
            _mediator.Setup(x => x.SendAsync(It.IsAny<UnlockUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>
            {
                { "Email", emailError },
                { "UnlockCode", unlockCodeError }
            }));

            //Act
            var actual = await _accountOrchestrator.UnlockUser(new UnlockUserViewModel());

            //Assert
            Assert.AreEqual(unlockCodeError, actual.Data.UnlockCodeError);
            Assert.AreEqual(emailError, actual.Data.EmailError);
            
        }
    }
}
