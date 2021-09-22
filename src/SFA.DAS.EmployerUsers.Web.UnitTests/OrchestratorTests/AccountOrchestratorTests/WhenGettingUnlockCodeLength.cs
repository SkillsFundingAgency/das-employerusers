using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetUnlockCodeLength;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenGettingUnlockCodeLength
    {
        private const int UnlockCodeLength = 99;

        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetUnlockCodeQuery>()))
                .ReturnsAsync(new GetUnlockCodeResponse
                {
                    UnlockCodeLength = UnlockCodeLength
                });

            _owinWrapper = new Mock<IOwinWrapper>();
            _logger = new Mock<ILogger>();

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheUnlockCodeIsFoundUsingAMediatr()
        {
            var unlockCodeLength = await _accountOrchestrator.GetUnlockCodeLength();
            Assert.AreEqual(UnlockCodeLength, unlockCodeLength);
        }
    }
}
