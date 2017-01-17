using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenResendingUnlockCode
    {
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountOrchestrator _accountOrchestrator;
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
        public async Task ThenTheUnlockLockUserViewModelOrchestratorResponseIsReturned()
        {

            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<ResendUnlockCodeCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "Email", "Some Error" } }));
            var model = new UnlockUserViewModel();

            //Act
            var actual = await _accountOrchestrator.ResendUnlockCode(model);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<OrchestratorResponse<UnlockUserViewModel>>(actual);
            Assert.IsFalse(actual.Data.UnlockCodeSent);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledWithAAccountLockedEvent()
        {
            //Arrange
            var expectedEmail = "test@local.com";
            var model = new UnlockUserViewModel { Email = expectedEmail };

            //Act
            await _accountOrchestrator.ResendUnlockCode(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<ResendUnlockCodeCommand>(s => s.Email == expectedEmail)), Times.Once());
        }

        [Test]
        public async Task ThenAnInvalidRequestExceptionIsHandledAndTheErrorDictionaryIsPopulatedIfTheAccountLockedEventIsNotValid()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<ResendUnlockCodeCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "Email", "Some Error" } }));

            //Act
            var actual = await _accountOrchestrator.ResendUnlockCode(new UnlockUserViewModel());

            //Assert
            Assert.IsNotEmpty(actual.Data.ErrorDictionary);
            Assert.Contains(new KeyValuePair<string,string>("Email","Some Error"),actual.Data.ErrorDictionary);
        }

        [Test]
        public async Task ThenTheResendUnlockCodeSuccessFlagIsSetToTrueIfAValidMessageIsSent()
        {
            //Arrange
            var expectedEmail = "test@local.com";
            var model = new UnlockUserViewModel { Email = expectedEmail };

            //Act
            var actual = await _accountOrchestrator.ResendUnlockCode(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<ResendUnlockCodeCommand>(s => s.Email == expectedEmail)), Times.Once());
            Assert.IsTrue(actual.Data.UnlockCodeSent);
        }
    }
}
