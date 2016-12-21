using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenStartingRegistration
    {
        private const string ClientId = "MyClient";
        private const string ReturnUrl = "http://unit.test/";

        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<ILogger> _logger;
        private AccountOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetRelyingPartyQuery>(q => q.Id == ClientId)))
                .ReturnsAsync(new Domain.RelyingParty
                {
                    Id = ClientId,
                    ApplicationUrl = "http://unit.test"
                });

            _owinWrapper = new Mock<IOwinWrapper>();

            _logger = new Mock<ILogger>();

            _orchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAValidModel()
        {
            // Act
            var actual = await _orchestrator.StartRegistration(ClientId, ReturnUrl, false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenItShouldIncludeReturnUrlInModel()
        {
            // Act
            var actual = await _orchestrator.StartRegistration(ClientId, ReturnUrl, false);

            // Assert
            Assert.AreEqual(ReturnUrl, actual.ReturnUrl);
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidModelIfReturnUrlIsNotLocalAndDoesNotMatchClient()
        {
            // Act
            var actual = await _orchestrator.StartRegistration(ClientId, "http://some-other.domain", false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
        }

        [Test]
        public async Task ThenItShouldReturnInvalidModelIfReturnUrlIsNotLocalAndClientIdNotFound()
        {
            // Act
            var actual = await _orchestrator.StartRegistration("NotMyClient", ReturnUrl, false);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Valid);
        }

        [TestCase(ClientId, "http://some-other.domain")]
        [TestCase("NotMyClient", ReturnUrl)]
        [TestCase("NotMyClient", "http://some-other.domain")]
        public async Task ThenItShouldReturnValidModelIfReturnUrlIsLocalEvenIfClientIdAndReturnUrlInvalid(string clientId, string returnUrl)
        {
            // Act
            var actual = await _orchestrator.StartRegistration(clientId, returnUrl, true);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Valid);
        }
    }
}
