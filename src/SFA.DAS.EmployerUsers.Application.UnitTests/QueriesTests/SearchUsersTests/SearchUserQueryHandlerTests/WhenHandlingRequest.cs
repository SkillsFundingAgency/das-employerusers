using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.SearchUsers;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.SearchUsersTests.SearchUserQueryHandlerTests
{
    public class WhenHandlingRequest
    {
        private Users _users;
        private Mock<IUserRepository> _userRepository;
        private SearchUsersQueryHandler _handler;
        private SearchUsersQuery _query;

        [SetUp]
        public void Arrange()
        {
            _users = new Users { UserCount = 123, UserList = new[] { new User(), new User() } };

            _query = new SearchUsersQuery { Criteria = "xyz", PageNumber = 2, PageSize = 100 };

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.SearchUsers(_query.Criteria, _query.PageSize, _query.PageNumber)).ReturnsAsync(_users);

            _handler = new SearchUsersQueryHandler(_userRepository.Object);
        }

        [Test]
        public async Task ThenItShouldReturnMatchingUsers()
        {
            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.AreSame(_users.UserList, actual.Users);
            Assert.AreEqual(_users.UserCount, actual.RecordCount);
        }
    }
}
