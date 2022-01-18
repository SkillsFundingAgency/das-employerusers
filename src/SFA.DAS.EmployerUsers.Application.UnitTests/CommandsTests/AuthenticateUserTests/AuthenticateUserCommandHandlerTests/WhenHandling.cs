using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

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
        private EmployerUsersConfiguration _configuration;
        private Mock<IMediator> _mediator;
        private AuthenticateUserCommandHandler _commandHandler;
        private AuthenticateUserCommand _command;
        private Mock<ILogger> _logger;
        private Mock<IValidator<AuthenticateUserCommand>> _validator;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void Arrange()
        {
            _user = new User
            {
                Email = EmailAddress,
                Password = CorrectHashedPassword,
                Salt = CorrectSalt,
                PasswordProfileId = CorrectProfileId,
                FailedLoginAttempts = 1
            };
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetByEmailAddress(EmailAddress))
                .Returns(Task.FromResult(_user));

            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(s => s.VerifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            _passwordService.Setup(s => s.VerifyAsync(CorrectPassword, CorrectHashedPassword, CorrectSalt, CorrectProfileId))
                .Returns(Task.FromResult(true));

            _configuration = new EmployerUsersConfiguration
            {
                Account = new AccountConfiguration
                {
                    AllowedFailedLoginAttempts = 2
                }
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(m => m.PublishAsync(It.IsAny<IAsyncNotification>())).Returns(Task.FromResult<object>(null));

            _logger = new Mock<ILogger>();

            _validator = new Mock<IValidator<AuthenticateUserCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AuthenticateUserCommand>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            _auditService = new Mock<IAuditService>();

            _commandHandler = new AuthenticateUserCommandHandler(
                _userRepository.Object, 
                _passwordService.Object, 
                _configuration,
                _mediator.Object,
                _logger.Object,
                _validator.Object,
                _auditService.Object);

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
        public void ThenItShouldThrowAnAccountLockedExceptionWhenAccountIsAlreadyLocked()
        {
            // Arrange
            _user.IsLocked = true;

            // Act + Assert
            Assert.ThrowsAsync<AccountLockedException>(async () => await _commandHandler.Handle(_command));
        }

        [Test]
        public async Task ThenItShouldReturnNullWhenPasswordIsIncorrect()
        {
            // Arrange
            _command.Password = CorrectPassword + "-Wrong";
            _user.FailedLoginAttempts = 0;

            // Act
            var actual = await _commandHandler.Handle(_command);

            // Assert
            Assert.IsNull(actual);
        }

        [Test]
        public async Task ThenItShouldUpdateFailedLoginAttemptsBy1WhenPasswordIsIncorrect()
        {
            // Arrange
            _command.Password = CorrectPassword + "-Wrong";

            // Act
            try
            {
                await _commandHandler.Handle(_command);
            }
            catch (AccountLockedException)
            {
                // This is expected, but do not care for this test
            }

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.FailedLoginAttempts == 2)), Times.Once());
        }

        [Test]
        public async Task ThenItShouldUpdateIsLockedToTrueWhenPasswordIsIncorrectAndAllowedFailedAttemptsReached()
        {
            // Arrange
            _command.Password = CorrectPassword + "-Wrong";

            // Act
            try
            {
                await _commandHandler.Handle(_command);
            }
            catch (AccountLockedException)
            {
                // This is expected, but do not care for this test
            }

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.IsLocked)), Times.Once());
        }

        [Test]
        public void ThenItShouldThrowAnAccountLockedExceptionWhenPasswordIsIncorrectAndAllowedFailedAttemptsReached()
        {
            // Arrange
            _command.Password = CorrectPassword + "-Wrong";

            // Act + Assert
            Assert.ThrowsAsync<AccountLockedException>(async () => await _commandHandler.Handle(_command));
        }

        [Test]
        public async Task ThenItShouldPublishAnAccountLockedEventWhenPasswordIsIncorrectAndAllowedFailedAttemptsReached()
        {
            // Arrange
            _command.Password = CorrectPassword + "-Wrong";

            // Act
            try
            {
                await _commandHandler.Handle(_command);
            }
            catch (AccountLockedException)
            {
                // This is expected, but do not care for this test
            }

            // Assert
            _mediator.Verify(m => m.PublishAsync(It.Is<AccountLockedEvent>(e => e.User == _user)), Times.Once());
        }

        [Test]
        public async Task ThenItShouldUpdateFailedLoginAttemptsTo0WhenGreaterThan0AndCredentialsMatch()
        {
            // Act
            var actual = await _commandHandler.Handle(_command);

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.FailedLoginAttempts == 0)), Times.Once());
        }

        [Test]
        public void ThenTheRepositoryIsNotCalledWhenTheValidatorIsReturnsFalseAndAnInvalidRequestExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<AuthenticateUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { {"",""} } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async ()=> await _commandHandler.Handle(_command));

            //Assert
            _userRepository.Verify(x=>x.GetByEmailAddress(It.IsAny<string>()),Times.Never);
        }

    }
}
