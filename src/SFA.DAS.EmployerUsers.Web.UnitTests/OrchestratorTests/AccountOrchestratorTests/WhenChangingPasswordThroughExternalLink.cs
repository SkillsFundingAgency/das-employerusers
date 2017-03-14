using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByHashedId;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserById;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenChangingPasswordThroughExternalLink
    {
        private const string HashedUserID = "ABCCV456";
        
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private AccountOrchestrator _orchestrator;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.SendAsync(It.Is<GetUserByHashedIdQuery>(q => q.HashedUserId == HashedUserID)))
                .Returns(Task.FromResult(new Domain.User
                {
                    Id = "1234RDF",
                    Email = "test@test.com"
                }));

            _owinWrapper = new Mock<IOwinWrapper>();

            _logger = new Mock<ILogger>();

            _orchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object, _logger.Object);

        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetTheUser()
        {
            //Act
            await _orchestrator.ForgottenPasswordFromEmail(HashedUserID);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetUserByHashedIdQuery>(c=>c.HashedUserId.Equals(HashedUserID))));
        }

        [Test]
        public async Task ThenIfTheMediatorReturnsNullThenABadRequestIsReturned()
        {
            //Act
            var actual = await _orchestrator.ForgottenPasswordFromEmail("123456");

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
        }
    }
}
