using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.ChangeEmail;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenConfirmingChangeEmail
    {
        private const string UserId = "USER1";
        private const string SecurityCode = "1A2B3C";
        private const string Password = "password";
        private const string ReturnUrl = "http://unit.test";

        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountOrchestrator _orchestrator;
        private ConfirmChangeEmailViewModel _model;
        private User _user;

        [SetUp]
        public void Arrange()
        {
            _user = new User
            {
                Id = UserId
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == UserId)))
                .Returns(Task.FromResult(_user));
            _mediator.Setup(m => m.SendAsync(It.IsAny<ChangeEmailCommand>()))
                .ThrowsAsync(new Exception("Called mediator with incorrect ChangeEmailCommand"));
            _mediator.Setup(m => m.SendAsync(It.Is<ChangeEmailCommand>(c => c.User == _user && c.SecurityCode == SecurityCode && c.Password == Password)))
                .Returns(Task.FromResult(new ChangeEmailCommandResult
                {
                    ReturnUrl = ReturnUrl
                }));

            _owinWrapper = new Mock<IOwinWrapper>();

            _orchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object);

            _model = new ConfirmChangeEmailViewModel
            {
                UserId = UserId,
                SecurityCode = SecurityCode,
                Password = Password
            };
        }

        [Test]
        public async Task ThenItShouldReturnAValidModelIfNoProblems()
        {
            // Act
            var actual = await _orchestrator.ConfirmChangeEmail(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenItShouldReturnTheReturnUrlForTheChange()
        {
            // Act
            var actual = await _orchestrator.ConfirmChangeEmail(_model);

            // Assert
            Assert.AreEqual(ReturnUrl, actual.ReturnUrl);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidModelWithAnErrorIfRequestIsInvalid()
        {
            // Arrange
            var errorMessages = new Dictionary<string, string> { { "", "Error" } };
            _mediator.Setup(m => m.SendAsync(It.IsAny<ChangeEmailCommand>()))
                .ThrowsAsync(new InvalidRequestException(errorMessages));

            // Act
            var actual = await _orchestrator.ConfirmChangeEmail(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
            Assert.IsNotNull(actual.ErrorDictionary);
            Assert.AreSame(errorMessages, actual.ErrorDictionary);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidModelWithAnErrorIfRequestErrors()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<ChangeEmailCommand>()))
                .ThrowsAsync(new Exception("A problem has occured"));

            // Act
            var actual = await _orchestrator.ConfirmChangeEmail(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
            Assert.IsNotNull(actual.ErrorDictionary);
            Assert.IsTrue(actual.ErrorDictionary.ContainsKey(""));
            Assert.AreEqual("A problem has occured", actual.ErrorDictionary[""]);
        }

    }
}
