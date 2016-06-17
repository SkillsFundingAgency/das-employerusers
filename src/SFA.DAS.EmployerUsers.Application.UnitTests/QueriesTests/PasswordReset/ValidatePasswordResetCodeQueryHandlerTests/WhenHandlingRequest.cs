using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Queries.IsPasswordResetValid;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.QueriesTests.PasswordReset.ValidatePasswordResetCodeQueryHandlerTests
{
    public class WhenHandlingRequest
    {
        private const string ActualEmailAddress = "someuser@local";
        private const string ExpiredEmailAddress = "someeexpired@local";
        public string PasswordResetCode = "123456ABC";
        private IsPasswordResetCodeValidQueryHandler _isPasswordResetCodeValidQueryHandler;
        private Mock<IUserRepository> _userRepository;

        [SetUp]
        public void Arrange()
        {
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>())).ReturnsAsync(null);
            _userRepository.Setup(x => x.GetByEmailAddress(ActualEmailAddress)).ReturnsAsync(new User {PasswordResetCode = PasswordResetCode});
            _userRepository.Setup(x => x.GetByEmailAddress(ExpiredEmailAddress)).ReturnsAsync(new User {PasswordResetCode = PasswordResetCode,PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(-1)});

            _isPasswordResetCodeValidQueryHandler = new IsPasswordResetCodeValidQueryHandler(_userRepository.Object);
        }

        [Test]
        public async Task ThenItShouldReturnABoolean()
        {
            //Arrange
            var query = new IsPasswordResetCodeValidQuery ();

            //Act
            var actual = await _isPasswordResetCodeValidQueryHandler.Handle(query);

            //Assert
            Assert.IsAssignableFrom<PasswordResetCodeResponse>(actual);
        }

        [Test]
        public async Task ThenTheUserIsRetrievedByTheEmailAddress()
        {
            //Arrange
            var query = new IsPasswordResetCodeValidQuery { Email = ActualEmailAddress };

            //Act
            await _isPasswordResetCodeValidQueryHandler.Handle(query);

            //Assert
            _userRepository.Verify(x => x.GetByEmailAddress(ActualEmailAddress), Times.Once);
        }

        [Test]
        public async Task ThenFalseIsReturnedIfTheUserDoesNotExist()
        {
            //Arrange
            var nonUser = "somenonexistantuser@local";
            var query = new IsPasswordResetCodeValidQuery { Email = nonUser };

            //Act
            var actual = await _isPasswordResetCodeValidQueryHandler.Handle(query);

            //Assert
            Assert.IsFalse(actual.IsValid);
        }

        [Test]
        public async Task ThenTrueIsReturnedIfTheCodeMatches()
        {
            //Arrange
            var query = new IsPasswordResetCodeValidQuery { Email = ActualEmailAddress, PasswordResetCode = PasswordResetCode };

            //Act
            var actual = await _isPasswordResetCodeValidQueryHandler.Handle(query);

            //Assert
            Assert.IsTrue(actual.IsValid);
            Assert.IsFalse(actual.HasExpired);
        }


        [Test]
        public async Task ThenTrueIsReturnedIfTheCodeMatchesButIsADifferentCase()
        {
            //Arrange
            var query = new IsPasswordResetCodeValidQuery { Email = ActualEmailAddress, PasswordResetCode = "123456abc" };

            //Act
            var actual = await _isPasswordResetCodeValidQueryHandler.Handle(query);

            //Assert
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public async Task ThenFalseIsReturnedIfThePasswordResetCodeHasExpiredForValidAndTrueForExpiredProperties()
        {
            //Arrange
            var query = new IsPasswordResetCodeValidQuery { Email = ExpiredEmailAddress, PasswordResetCode = PasswordResetCode };

            //Act
            var actual = await _isPasswordResetCodeValidQueryHandler.Handle(query);

            //Assert
            Assert.IsFalse(actual.IsValid);
            Assert.IsTrue(actual.HasExpired);
        }


        [Test]
        public async Task ThenFalseIsReturnedIfTheCodeDoesNotMatch()
        {
            //Arrange
            var query = new IsPasswordResetCodeValidQuery { Email = ActualEmailAddress, PasswordResetCode = "654321" };

            //Act
            var actual = await _isPasswordResetCodeValidQueryHandler.Handle(query);

            //Assert
            Assert.IsFalse(actual.IsValid);
            Assert.IsFalse(actual.HasExpired);
        }

    }
}
