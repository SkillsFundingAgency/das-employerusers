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
    public class WhenStartingChangePassword
    {
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<ILogger> _logger;
        private AccountOrchestrator _orchestrator;

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

            _owinWrapper = new Mock<IOwinWrapper>();

            _logger = new Mock<ILogger>();

            _orchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);
        }

        [TestCase("MyClient", "http://unit.test")]
        [TestCase("MyClient", "http://unit.test/")]
        [TestCase("MyClient", "http://unit.test/some/path")]
        public async Task ThenItShouldReturnAValidModelIfReturnUrlValidForClientId(string requestedClientId, string requestedReturnUrl)
        {
            // Act
            var actual = await _orchestrator.StartChangePassword(requestedClientId, requestedReturnUrl);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Data.Valid);
        }

        [Test]
        public async Task ThenItShouldReturnInvalidModelIfClientIdNotFound()
        {
            // Act
            var actual = await _orchestrator.StartChangePassword("NotMyClient", "http://unit.test");

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.Valid);
        }

        [TestCase("http://sub.unit.test")]
        [TestCase("https://unit.test")]
        [TestCase("https://another.domain")]
        public async Task ThenItShouldReturnInvalidModelIfReturnUrlNotValidForClientId(string returnUrl)
        {
            // Act
            var actual = await _orchestrator.StartChangePassword("MyClient", returnUrl);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.Data.Valid);
        }
    }
}
