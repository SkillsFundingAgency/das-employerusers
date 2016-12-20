using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.RequestChangeEmail;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenRequestingChangeEmail
    {
        private const string UserId = "USER1";
        private const string EmailAddress = "user.one@unit.test";
        private const string ReturnUrl = "http://unit.test";

        private ChangeEmailViewModel _model;
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountOrchestrator _orchestrator;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _model = new ChangeEmailViewModel
            {
                UserId = UserId,
                NewEmailAddress = EmailAddress,
                ConfirmEmailAddress = EmailAddress,
                ClientId = "MyClient",
                ReturnUrl = ReturnUrl
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetRelyingPartyQuery>(q => q.Id == "MyClient")))
                .ReturnsAsync(new Domain.RelyingParty
                {
                    Id = "MyClient",
                    ApplicationUrl = "http://unit.test"
                });
            _mediator.Setup(m => m.SendAsync(ItIsRequestChangeEmailCommandForModel()))
                .Returns(Task.FromResult(Unit.Value));

            _owinWrapper = new Mock<IOwinWrapper>();
            _logger = new Mock<ILogger>();

            _orchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAnInstanceOfChangeEmailViewModel()
        {
            // Act
            var actual = await _orchestrator.RequestChangeEmail(_model);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnValidIfNoErrors()
        {
            // Act
            var actual = await _orchestrator.RequestChangeEmail(_model);

            // Assert
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenItShouldReturnInvalidAndErrorDetailsIfValidationErrorsOccurs()
        {
            //Arrange
            _mediator.Setup(m => m.SendAsync(ItIsRequestChangeEmailCommandForModel()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "ConfirmEmailAddress", "Error" } }));

            // Act
            var actual = await _orchestrator.RequestChangeEmail(_model);

            // Assert
            Assert.IsFalse(actual.Valid);
            Assert.IsNotNull(actual.ErrorDictionary);
            Assert.IsTrue(actual.ErrorDictionary.ContainsKey("ConfirmEmailAddress"));
        }

        [Test]
        public async Task ThenItShouldReturnInvalidAndErrorDetailsIfNonValidationErrorsOccurs()
        {
            //Arrange
            _mediator.Setup(m => m.SendAsync(ItIsRequestChangeEmailCommandForModel()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var actual = await _orchestrator.RequestChangeEmail(_model);

            // Assert
            Assert.IsFalse(actual.Valid);
            Assert.IsNotNull(actual.ErrorDictionary);
            Assert.IsTrue(actual.ErrorDictionary.ContainsKey(""));
            Assert.AreEqual("Error", actual.ErrorDictionary[""]);
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
            var actual = await _orchestrator.RequestChangeEmail(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenItShouldReturnInvalidModelIfClientIdNotFound()
        {
            // Arrange
            _model.ClientId = "NotMyClient";
            _model.ReturnUrl = "http://unit.test";

            // Act
            var actual = await _orchestrator.RequestChangeEmail(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
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
            var actual = await _orchestrator.RequestChangeEmail(_model);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
        }


        private RequestChangeEmailCommand ItIsRequestChangeEmailCommandForModel()
        {
            return It.Is<RequestChangeEmailCommand>(c => c.UserId == _model.UserId
                                                      && c.NewEmailAddress == _model.NewEmailAddress
                                                      && c.ConfirmEmailAddress == _model.ConfirmEmailAddress
                                                      && c.ReturnUrl == ReturnUrl);
        }
    }
}
