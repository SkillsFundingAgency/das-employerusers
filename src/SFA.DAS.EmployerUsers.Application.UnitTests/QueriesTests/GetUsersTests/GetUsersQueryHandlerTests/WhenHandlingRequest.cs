using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.GetUsers;
using SFA.DAS.EmployerUsers.Application.Queries.SearchUsers;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.GetUsersTests.GetUsersQueryHandlerTests
{
    public class WhenHandlingRequest
    {
        private User[] _users;
        private Mock<IUserRepository> _userRepository;
        private GetUsersQueryHandler _handler;
        private GetUsersQuery _query;

        [SetUp]
        public void Arrange()
        {
            _users = new[] { new User(), new User() };

            _query = new GetUsersQuery { PageSize = 200, PageNumber = 2 };

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetUsers(_query.PageSize, _query.PageNumber)).ReturnsAsync(_users);
            _userRepository.Setup(r => r.GetUserCount()).ReturnsAsync(_users.Length);

            _handler = new GetUsersQueryHandler(_userRepository.Object);
        }

        [Test]
        public async Task ThenItShouldReturnTheUsers()
        {
            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.AreSame(_users, actual.Users);
            Assert.AreEqual(_users.Length, actual.RecordCount);
        }
    }
}
