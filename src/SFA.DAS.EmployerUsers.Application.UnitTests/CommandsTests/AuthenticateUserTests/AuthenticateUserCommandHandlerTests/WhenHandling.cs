using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.AuthenticateUserTests.AuthenticateUserCommandHandlerTests
{
    public class WhenHandling
    {
        private const string CorrectPassword = "password";
        private const string CorrectHashedPassword = "azYot9HLQpd7lBccbJtvV8rqeJdDJ7YuJVfty2V2H9BJE446ove5AXYwvWY70V/yQm0MKh7MAhd/bNgVc8zb5ThqwFp8OWHEOEDx/grhhlphXKaI0KqZrlxOHConhI+Qop0aowpB3+7CsVqsRbrT637BRdt2LDQg/P92K3sucQQfVDN/crrIUxLzxWEWoNQ3zUpcXoCfghe4Hulz6A6lKgTrBlEVbdJhTuGSb/0nAP1sS4HXj2H66CGRjXYpEA2X2pPhJuQk7Os04AYxey3AUz3UgTvmFFzEFxzG5ugbqrx1TE5wBYHuVo6cVcCp93+v79oo7eB8lrqKeNmzOOtQCA==";
        private const string CorrectSalt = "H9neDfu0bUWWjHa2XPL9/w==";
        private const string CorrectProfileId = "XYZ";
        private const string EmailAddress = "unit.tests@test.local";

        private User _user;
        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordService> _passwordService;
        private AuthenticateUserCommandHandler _commandHandler;
        private AuthenticateUserCommand _command;

        [SetUp]
        public void Arrange()
        {
            _user = new Domain.User
            {
                Email = EmailAddress,
                Password = CorrectHashedPassword,
                Salt = CorrectSalt,
                PasswordProfileId = CorrectProfileId
            };
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetByEmailAddress(EmailAddress))
                .Returns(Task.FromResult(_user));

            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(s => s.VerifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            _passwordService.Setup(s=>s.VerifyAsync(CorrectPassword, CorrectHashedPassword, CorrectSalt, CorrectProfileId))
                .Returns(Task.FromResult(true));

            _commandHandler = new AuthenticateUserCommandHandler(_userRepository.Object, _passwordService.Object);

            _command = new AuthenticateUserCommand
            {
                EmailAddress = EmailAddress,
                Password = CorrectPassword
            };
        }

        [Test]
        public async Task ThenItShouldReturnUserWhenCredentialsMatch()
        {
            // Act
            var actual = await _commandHandler.Handle(_command);

            // Assert
            Assert.AreSame(_user, actual);
        }

        [Test]
        public async Task ThenItShouldReturnNullWhenUserIsNotFound()
        {
            // Arrange
            _command.EmailAddress = EmailAddress + "-Wrong";

            // Act
            var actual = await _commandHandler.Handle(_command);

            // Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnNullWhenPasswordIsIncorrect()
        {
            // Arrange
            _command.Password = CorrectPassword + "-Wrong";

            // Act
            var actual = await _commandHandler.Handle(_command);

            // Assert
            Assert.IsNull(actual);
        }
        
    }
}
