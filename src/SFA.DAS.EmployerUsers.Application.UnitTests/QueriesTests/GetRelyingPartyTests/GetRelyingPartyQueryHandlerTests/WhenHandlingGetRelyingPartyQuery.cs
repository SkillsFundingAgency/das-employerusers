using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetRelyingParty;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.GetRelyingPartyTests.GetRelyingPartyQueryHandlerTests
{
    public class WhenHandlingGetRelyingPartyQuery
    {
        private RelyingParty _relyingParty;
        private Mock<IRelyingPartyRepository> _relyingPartyRepository;
        private GetRelyingPartyQueryHandler _handler;
        private GetRelyingPartyQuery _query;

        [SetUp]
        public void Arrange()
        {
            _relyingParty = new RelyingParty
            {
                Id = "MyClient"
            };
            _relyingPartyRepository = new Mock<IRelyingPartyRepository>();
            _relyingPartyRepository.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new[]
                {
                    _relyingParty
                });

            _handler = new GetRelyingPartyQueryHandler(_relyingPartyRepository.Object);

            _query = new GetRelyingPartyQuery
            {
                Id = "MyClient"
            };
        }

        [TestCase("MyClient")]
        [TestCase("myclient")]
        public async Task ThenItShouldReturnRelyingPartyFromRepositoryIfFound(string requestedId)
        {
            // Arrange
            _query.Id = requestedId;

            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.AreSame(_relyingParty, actual);
        }

        [Test]
        public async Task ThenItShouldReturnNullIfNoRelyingPartyFound()
        {
            // Arrange
            _query.Id = "NotMyClient";

            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.IsNull(actual);
        }
    }
}
