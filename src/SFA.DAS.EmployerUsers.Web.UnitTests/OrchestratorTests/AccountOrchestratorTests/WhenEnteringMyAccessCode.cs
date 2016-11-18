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
        private const string UserId = "User1";
        private const string AccessCode = "123ABC";
        private const string ReturnUrl = "http://unit.test";

        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private ActivateUserViewModel _model;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<ActivateUserCommand>()))
                .ReturnsAsync(new ActivateUserCommandResult
                {
                    ReturnUrl = ReturnUrl
                });

            _owinWrapper = new Mock<IOwinWrapper>();

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object);

            _model = new ActivateUserViewModel
            {
                UserId = UserId,
                AccessCode = AccessCode
            };
        }


        [Test]
        public async Task ThenTheActivateUserCommandIsPassedOntoTheMediator()
        {
            //Act
            var actual = await _accountOrchestrator.ActivateUser(_model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<ActivateUserCommand>(p => p.AccessCode.Equals(AccessCode) && p.UserId.Equals(UserId))), Times.Once);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenFalseIsReturnedWhenTheRegisterUserCommandHandlerThrowsAnException()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<ActivateUserCommand>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _accountOrchestrator.ActivateUser(_model);

            //Assert
            Assert.IsFalse(actual.Valid);
        }

        [Test]
        public async Task ThenReturlUrlIsReturned()
        {
            // Act
            var actual = await _accountOrchestrator.ActivateUser(_model);

            // Assert
            Assert.AreEqual(ReturnUrl, actual.ReturnUrl);
        }
    }
}
