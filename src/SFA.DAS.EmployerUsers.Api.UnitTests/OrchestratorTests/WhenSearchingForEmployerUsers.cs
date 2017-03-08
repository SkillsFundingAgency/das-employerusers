using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Api.Orchestrators;
using SFA.DAS.EmployerUsers.Application.Queries.SearchUsers;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Api.UnitTests.OrchestratorTests
{
    [TestFixture]
    public sealed class WhenSearchingForEmployerUsers
    {
        private Mock<IMediator> _mediator;
        private SearchOrchestrator _orchestrator;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            
            _logger = new Mock<ILogger>();
            
            _orchestrator = new SearchOrchestrator(_mediator.Object, _logger.Object);
        }

        [TestCase(9, 3, 3)]
        [TestCase(11, 3, 4)]
        [TestCase(5, 10, 1)]
        [TestCase(50, 10, 5)]
        [TestCase(10, 10, 1)]
        public async Task ThenTotalPagesCountShouldTakeAccountOfNoOfResultsAndPageSize(int noOfResults, int pageSize, int expectedTotalPages)
        {
            // Arrange
            _mediator.Setup(m => m.SendAsync(It.Is<SearchUsersQuery>(q => q.Criteria == "Joe"
                                                                    && q.PageSize == pageSize
                                                                    && q.PageNumber == 1)))
                .ReturnsAsync(new SearchUsersQueryResponse
                {
                    RecordCount = noOfResults,
                    Users = Enumerable.Repeat(new User { Id = "1", FirstName = "Joe", LastName = "Bloggs" }, noOfResults).ToArray()
                });

            // Act
            var actual = await _orchestrator.UserSearch("Joe", pageSize, 1);
            
            // Assert
            Assert.IsNotNull(actual);
            actual.Data.TotalPages.Should().Be(expectedTotalPages);
        }
    }
}
