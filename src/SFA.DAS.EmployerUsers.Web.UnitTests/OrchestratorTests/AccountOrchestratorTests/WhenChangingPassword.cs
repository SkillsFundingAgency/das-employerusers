using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ChangePassword;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenChangingPassword
    {
        private const string UserId = "User1";
        private const string CurrentPassword = "Password";
        private const string NewPassword = "NewPassword";

        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountOrchestrator _orchestrator;
        private ChangePasswordViewModel _model;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetRelyingPartyQuery>(q => q.Id == "MyClient")))
                .ReturnsAsync(new Domain.RelyingParty
                {
                    Id = "MyClient",
                    ApplicationUrl = "http://unit.test"
                });
            _mediator.Setup(m => m.SendAsync(It.Is<GetUserByIdQuery>(q => q.UserId == UserId)))
                .Returns(Task.FromResult(new Domain.User
                {
                    Id = UserId
                }));

            _owinWrapper = new Mock<IOwinWrapper>();

            _logger = new Mock<ILogger>();

            _orchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);

            _model = new ChangePasswordViewModel
            {
                UserId = UserId,
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = NewPassword,
                ClientId = "MyClient",
                ReturnUrl = "http://unit.test"
            };
        }

        [Test]
        public async Task ThenItShouldReturnAValidModelIfChangeSuccessful()
        {
            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Data.Valid);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidModelWithErrorsIfAValidationErrorOccurs()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<ChangePasswordCommand>(c => c.User.Id == UserId
                                                                            && c.CurrentPassword == CurrentPassword
                                                                            && c.NewPassword == NewPassword
                                                                            && c.ConfirmPassword == NewPassword)))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "", "Not valid" } }));
            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.Valid);
            Assert.IsNotNull(actual.Data.ErrorDictionary);
            Assert.IsTrue(actual.Data.ErrorDictionary.ContainsKey(""));
            Assert.AreEqual("Not valid", actual.Data.ErrorDictionary[""]);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidModelWithAnErrorIfRequestErrors()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<ChangePasswordCommand>()))
                .ThrowsAsync(new Exception("A problem has occured"));

            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.Valid);
            Assert.IsNotNull(actual.Data.ErrorDictionary);
            Assert.IsTrue(actual.Data.ErrorDictionary.ContainsKey(""));
            Assert.AreEqual("A problem has occured", actual.Data.ErrorDictionary[""]);
        }

        [Test]
        public async Task ThenItShouldReturnModelWithPasswordsBlankedIfRequestNotValid()
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.IsAny<ChangePasswordCommand>()))
                .ThrowsAsync(new Exception("A problem has occured"));

            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual.Data);
            Assert.IsEmpty(actual.Data.CurrentPassword);
            Assert.IsEmpty(actual.Data.NewPassword);
            Assert.IsEmpty(actual.Data.ConfirmPassword);
        }

        [TestCase("MyClient", "http://unit.test")]
        [TestCase("MyClient", "http://unit.test/")]
        [TestCase("MyClient", "http://unit.test/some/path")]
        public async Task ThenItShouldReturnAValidModelIfReturnUrlValidForClientId(string requestedClientId, string requestedReturnUrl)
        {
            // Arrange
            _model.ClientId = requestedClientId;
            _model.ReturnUrl = requestedReturnUrl;

            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Data.Valid);
        }

        [Test]
        public async Task ThenItShouldReturnInvalidModelIfClientIdNotFound()
        {
            // Arrange
            _model.ClientId = "NotMyClient";
            _model.ReturnUrl = "http://unit.test";

            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.Valid);
        }

        [TestCase("http://sub.unit.test")]
        [TestCase("https://unit.test")]
        [TestCase("https://another.domain")]
        public async Task ThenItShouldReturnInvalidModelIfReturnUrlNotValidForClientId(string returnUrl)
        {
            // Arrange
            _model.ClientId = "MyClient";
            _model.ReturnUrl = returnUrl;

            // Act
            var actual = await _orchestrator.ChangePassword(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.Valid);
        }
    }
}
