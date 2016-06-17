using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.UnitTests.TestHelpers;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RequestPasswordResetCodeTests.RequestPasswordResetCodeCommandTests
{
    [TestFixture]
    public class WhenHandlingTheCommand
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<ICommunicationService> _communicationSerivce;
        private Mock<ICodeGenerator> _codeGenerator;
        private RequestPasswordResetCodeCommandHandler _commandHandler;

        [SetUp]
        public void Setup()
        {
            _userRepository = new Mock<IUserRepository>();
            _communicationSerivce = new Mock<ICommunicationService>();
            _codeGenerator = new Mock<ICodeGenerator>();
            _commandHandler = new RequestPasswordResetCodeCommandHandler(new RequestPasswordResetCodeCommandValidator(), _userRepository.Object, _communicationSerivce.Object, _codeGenerator.Object);
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public void InvalidCommandThrowsInvalidRequestException()
        {
            var command = new RequestPasswordResetCodeCommand();

            var invalidRequestException = Assert.ThrowsAsync<InvalidRequestException>(() => _commandHandler.Handle(command));

            Assert.That(invalidRequestException.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(invalidRequestException.ErrorMessages.Keys.Contains("Email"));
        }

        [Test]
        public void UnknownEmailThrowsInvalidRequestException()
        {
            var command = GetRequestPasswordResetCodeCommand();

            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync(null);

            var invalidRequestException = Assert.ThrowsAsync<InvalidRequestException>(() => _commandHandler.Handle(command));

            Assert.That(invalidRequestException.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(invalidRequestException.ErrorMessages.Keys.Contains("UserNotFound"));
        }

        [Test]
        public async Task KnownUserWithActiveResetResendsExistingCode()
        {
            var command = GetRequestPasswordResetCodeCommand();

            var existingUser = new User
            {
                Email = command.Email,
                PasswordResetCode = "ABCDEF",
                PasswordResetCodeExpiry = DateTimeProvider.Current.UtcNow.AddHours(1)
            };

            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync(existingUser);

            await _commandHandler.Handle(command);

            _communicationSerivce.Verify(x => x.SendPasswordResetCodeMessage(It.Is<User>(u => u.Email == existingUser.Email && u.PasswordResetCode == existingUser.PasswordResetCode && u.PasswordResetCodeExpiry == existingUser.PasswordResetCodeExpiry), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task KnownUserWithExpiredCodeSendsNewCode()
        {
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.UtcNow);
            const string newCode = "FEDCBA";

            var command = GetRequestPasswordResetCodeCommand();

            var existingUser = new User
            {
                Email = command.Email,
                PasswordResetCode = "ABCDEF",
                PasswordResetCodeExpiry = DateTimeProvider.Current.UtcNow.AddHours(-1)
            };

            _codeGenerator.Setup(x => x.GenerateAlphaNumeric(6)).Returns(newCode);

            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync(existingUser);

            await _commandHandler.Handle(command);

            _communicationSerivce.Verify(x => x.SendPasswordResetCodeMessage(It.Is<User>(u => u.Email == existingUser.Email && u.PasswordResetCode == newCode && u.PasswordResetCodeExpiry == DateTimeProvider.Current.UtcNow.AddDays(1)), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task KnownUserWithNoCodeButExpiryDateSendsNewCode()
        {
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.UtcNow);
            const string newCode = "FEDCBA";

            var command = GetRequestPasswordResetCodeCommand();

            var existingUser = new User
            {
                Email = command.Email,
                PasswordResetCode = "",
                PasswordResetCodeExpiry = DateTimeProvider.Current.UtcNow.AddHours(1)
            };

            _codeGenerator.Setup(x => x.GenerateAlphaNumeric(6)).Returns(newCode);

            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync(existingUser);

            await _commandHandler.Handle(command);

            _communicationSerivce.Verify(x => x.SendPasswordResetCodeMessage(It.Is<User>(u => u.Email == existingUser.Email && u.PasswordResetCode == newCode && u.PasswordResetCodeExpiry == DateTimeProvider.Current.UtcNow.AddDays(1)), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task KnownUserWithCodeButNoExpiryDateSendsNewCode()
        {
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.UtcNow);
            const string newCode = "FEDCBA";

            var command = GetRequestPasswordResetCodeCommand();

            var existingUser = new User
            {
                Email = command.Email,
                PasswordResetCode = "ABCDEF",
                PasswordResetCodeExpiry = null
            };

            _codeGenerator.Setup(x => x.GenerateAlphaNumeric(6)).Returns(newCode);

            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync(existingUser);

            await _commandHandler.Handle(command);

            _communicationSerivce.Verify(x => x.SendPasswordResetCodeMessage(It.Is<User>(u => u.Email == existingUser.Email && u.PasswordResetCode == newCode && u.PasswordResetCodeExpiry == DateTimeProvider.Current.UtcNow.AddDays(1)), It.IsAny<string>()), Times.Once);
        }

        private RequestPasswordResetCodeCommand GetRequestPasswordResetCodeCommand()
        {
            return new RequestPasswordResetCodeCommand
            {
                Email = "test.user@test.org"
            };
        }
    }
}