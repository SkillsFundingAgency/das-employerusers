using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.EventsTests.AccountLockedTests.GenerateAndEmailAccountLockedEmailHandlerTests
{
    public class WhenHandling
    {
        private const string UnlockCode = "ABC123";

        private Mock<IConfigurationService> _configurationService;
        private User _user;
        private Mock<IUserRepository> _userRepository;
        private Mock<ICodeGenerator> _codeGenerator;
        private Mock<ICommunicationService> _communicationService;
        private GenerateAndEmailAccountLockedEmailHandler _handler;
        private AccountLockedEvent _event;

        [SetUp]
        public void Arrange()
        {
            _configurationService = new Mock<IConfigurationService>();
            _configurationService.Setup(c => c.GetAsync<EmployerUsersConfiguration>())
                .Returns(Task.FromResult(new EmployerUsersConfiguration
                {
                    Account = new AccountConfiguration
                    {
                        UnlockCodeLength = 10
                    }
                }));

            _user = new User
            {
                Id = "xxx",
                Email = "unit.tests@testing.local"
            };
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetById("xxx")).Returns(Task.FromResult(_user));

            _codeGenerator = new Mock<ICodeGenerator>();
            _codeGenerator.Setup(cg => cg.GenerateAlphaNumeric(10)).Returns(UnlockCode);

            _communicationService = new Mock<ICommunicationService>();
            _communicationService.Setup(s => s.SendAccountLockedMessage(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.FromResult<object>(null));

            _handler = new GenerateAndEmailAccountLockedEmailHandler(
                _configurationService.Object,
                _userRepository.Object,
                _codeGenerator.Object,
                _communicationService.Object);

            _event = new AccountLockedEvent
            {
                User = new User
                {
                    Id = _user.Id
                }
            };
        }

        [Test]
        public async Task ThenItShouldUpdateUserWithNewlyGeneratedCodeIfOneNotAttached()
        {
            // Act
            await _handler.Handle(_event);

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.UnlockCode == UnlockCode)), Times.Once());
        }

        [Test]
        public async Task ThenItShouldNotUpdateUserWithNewlyGeneratedCodeIfOneAlreadyAttached()
        {
            // Arrange
            _user.UnlockCode = "UNLOCK_CODE";

            // Act
            await _handler.Handle(_event);

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.UnlockCode == UnlockCode)), Times.Never());
        }

        [Test]
        public async Task ThenItShouldSendUserNotificationIfNewCodeGenerated()
        {
            // Act
            await _handler.Handle(_event);

            // Assert
            _communicationService.Verify(s => s.SendAccountLockedMessage(_user, It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ThenItShouldNotSendUserNotificationIfNewCodeNotGenerated()
        {
            // Arrange
            _user.UnlockCode = "UNLOCK_CODE";

            // Act
            await _handler.Handle(_event);

            // Assert
            _communicationService.Verify(s => s.SendAccountLockedMessage(_user, It.IsAny<string>()), Times.Never());
        }
    }
}
