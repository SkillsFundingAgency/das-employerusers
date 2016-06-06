using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenEnteringMyAccessCode
    {
        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _accountOrchestrator = new AccountOrchestrator(_mediator.Object,_owinWrapper.Object);
        }


        [Test]
        public async Task ThenABooleanValueIsReturned()
        {
            //Arrange
            var accessCodeviewModel = new AccessCodeViewModel();

            //Act
            var actual = await _accountOrchestrator.ActivateUser(accessCodeviewModel);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<bool>(actual);
        }

        [Test]
        public async Task ThenTheActivateUserCommandIsPassedOntoTheMediator()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var accessCode = "MyCode";
            var accessCodeviewModel = new AccessCodeViewModel { AccessCode = accessCode, UserId = userId };

            //Act
            var actual = await _accountOrchestrator.ActivateUser(accessCodeviewModel);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<ActivateUserCommand>(p => p.AccessCode.Equals(accessCode) && p.UserId.Equals(userId))), Times.Once);
            Assert.IsTrue(actual);
        }

        [Test]
        public async Task ThenFalseIsReturnedWhenTheRegisterUserCommandHandlerThrowsAnException()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<ActivateUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _accountOrchestrator.ActivateUser(new AccessCodeViewModel());

            //Assert
            Assert.IsFalse(actual);

        }
    }
}
