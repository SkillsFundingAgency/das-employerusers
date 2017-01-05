using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsersWithExpiredRegistrations;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.GetUsersWithExpiredRegistrationsTests.GetUsersWithExpiredRegistrationsQueryHandlerTests
{
    public class WhenHandlingQuery
    {
        private User _user;
        private Mock<IUserRepository> _userRepository;
        private GetUsersWithExpiredRegistrationsQueryHandler _handler;
        private GetUsersWithExpiredRegistrationsQuery _query;

        [SetUp]
        public void Arrange()
        {
            _user = new User
            {
                Id = "USER1"
            };

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetUsersWithExpiredRegistrations())
                .ReturnsAsync(new[]
                {
                    _user
                });

            _handler = new GetUsersWithExpiredRegistrationsQueryHandler(_userRepository.Object);

            _query = new GetUsersWithExpiredRegistrationsQuery();
        }

        [Test]
        public async Task ThenItShouldReturnArrayOfUsersFromRepo()
        {
            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Length);
            Assert.AreSame(_user, actual[0]);
        }
    }
}
