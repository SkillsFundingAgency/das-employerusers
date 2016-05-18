using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.ApplicationLayer.Queries.IsUserActive;
using SFA.DAS.EmployerUsers.Data.User;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.UnitTests.QueriesTests.IsUserActiveTests.IsUserActiveQueryHandlerTests
{
    public class WhenHandlingRequest
    {
        private User _user;
        private Mock<IUserRepository> _userRepository;
        private IsUserActiveQueryHandler _handler;
        private IsUserActiveQuery _query;

        [SetUp]
        public void Arrange()
        {
            _user = new User
            {
                IsActive = true
            };
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetById("xyz")).Returns(Task.FromResult(_user));

            _handler = new IsUserActiveQueryHandler(_userRepository.Object);

            _query = new IsUserActiveQuery { UserId = "xyz" };
        }

        [Test]
        public async Task ThenItShouldReturnTrueIfUserIsActive()
        {
            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public async Task ThenItShouldReturnFalseIfUserIsNotActive()
        {
            // Arrange
            _user.IsActive = false;

            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public async Task ThenItShouldReturnFalseIfUserIsNotAFound()
        {
            // Arrange
            _query.UserId = "zyx";

            // Act
            var actual = await _handler.Handle(_query);

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void ThenItShouldThrowExceptionIfQueryIsNull()
        {
            // Act + Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(null));
            Assert.AreEqual("message", ex.ParamName);
        }

    }
}
