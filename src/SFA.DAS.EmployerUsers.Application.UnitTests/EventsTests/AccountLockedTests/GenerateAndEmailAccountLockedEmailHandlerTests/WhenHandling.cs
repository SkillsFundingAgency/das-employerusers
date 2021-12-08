using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Events.AccountLocked;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.EventsTests.AccountLockedTests.GenerateAndEmailAccountLockedEmailHandlerTests
{
    public class WhenHandling
    {
        private const string UnlockCode = "ABC123";
        private const string UserEmail = "unit.tests@testing.local";

        private Mock<IConfigurationService> _configurationService;
        private User _user;
        private Mock<IUserRepository> _userRepository;
        private Mock<ICodeGenerator> _codeGenerator;
        private Mock<ICommunicationService> _communicationService;
        private GenerateAndEmailAccountLockedEmailHandler _handler;
        private AccountLockedEvent _event;
        private Mock<ILogger> _logger;
        private Mock<IAuditService> _auditService;
        private string _employerAccountsBaseUrl;

        [SetUp]
        public void Arrange()
        {
            _employerAccountsBaseUrl = "https://localhost:44344";
            _configurationService = new Mock<IConfigurationService>();
            _configurationService.Setup(c => c.GetAsync<EmployerUsersConfiguration>())
                .Returns(Task.FromResult(new EmployerUsersConfiguration
                {
                    Account = new AccountConfiguration
                    {
                        UnlockCodeLength = 10
                    },
                    EmployerAccountsBaseUrl = _employerAccountsBaseUrl
                }));            

            _user = new User
            {
                Id = "xxx",
                Email = UserEmail
            };
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetById("xxx")).Returns(Task.FromResult(_user));
            _userRepository.Setup(r => r.GetByEmailAddress(UserEmail)).Returns(Task.FromResult(_user));

            _codeGenerator = new Mock<ICodeGenerator>();
            _codeGenerator.Setup(cg => cg.GenerateAlphaNumeric(10)).Returns(UnlockCode);

            _communicationService = new Mock<ICommunicationService>();
            _communicationService.Setup(s => s.SendAccountLockedMessage(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.FromResult<object>(null));

            _logger = new Mock<ILogger>();

            _auditService = new Mock<IAuditService>();

            _handler = new GenerateAndEmailAccountLockedEmailHandler(
                _configurationService.Object,
                _userRepository.Object,
                _codeGenerator.Object,
                _communicationService.Object,
                _auditService.Object,
                _logger.Object);

            _event = new AccountLockedEvent
            {
                User = new User
                {
                    Id = _user.Id
                }
            };
        }

        [Test]
        public async Task ThenItShouldUpdateUserWithNewlyGeneratedCodeAndAnExpiryOf1DayIfNoCodeAttached()
        {
            //Arrange
            _event.ReturnUrl = "http://test.local";

            // Act
            await _handler.Handle(_event);

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.SecurityCodes.Any(sc => sc.Code == UnlockCode
                                                                                         && sc.CodeType == SecurityCodeType.UnlockCode
                                                                                         && sc.ExpiryTime.Date == DateTime.Today.AddDays(1)))));
        }

        [Test]
        public async Task ThenItShouldNotUpdateUserWithNewlyGeneratedCodeIfOneAlreadyAttachedAndNotExpired()
        {
            // Arrange
            _user.SecurityCodes = new[]
            {
                new SecurityCode
                {
                    Code = "UNLOCK_CODE",
                    CodeType = SecurityCodeType.UnlockCode,
                    ExpiryTime = DateTime.MaxValue
                }
            };

            // Act
            await _handler.Handle(_event);

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.SecurityCodes.Any(sc => sc.Code == UnlockCode && sc.CodeType == SecurityCodeType.UnlockCode))), Times.Never());
        }

        [Test]
        public async Task ThenItShouldSendUserNotificationIfNewCodeGenerated()
        {
            //Arrange
            _event.ReturnUrl = "http://test.local";

            // Act
            await _handler.Handle(_event);

            // Assert
            _communicationService.Verify(s => s.SendAccountLockedMessage(It.Is<User>(c => c.SecurityCodes.Any(sc => sc.Code == UnlockCode
                                                                                                                 && sc.CodeType == SecurityCodeType.UnlockCode)
                                                                                       && c.Id == _user.Id
                                                                                       && c.Email == _user.Email),
                                                                         It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ThenItShouldNotSendUserNotificationIfNewCodeNotGenerated()
        {
            // Arrange
            _user.SecurityCodes = new[]
            {
                new SecurityCode
                {
                    Code = "UNLOCK_CODE",
                    CodeType = SecurityCodeType.UnlockCode,
                    ExpiryTime = DateTime.MaxValue
                }
            };

            // Act
            await _handler.Handle(_event);

            // Assert
            _communicationService.Verify(s => s.SendAccountLockedMessage(_user, It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ThenItShouldGenerateANewCodeIfTheCurrentCodeHasExpired()
        {
            // Arrange
            _user.SecurityCodes = new[]
            {
                new SecurityCode
                {
                    Code = "UNLOCK_CODE",
                    CodeType = SecurityCodeType.UnlockCode,
                    ExpiryTime = DateTime.MinValue,
                    ReturnUrl = "http://test.local"
                }
            };

            // Act
            await _handler.Handle(_event);

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.SecurityCodes.Any(sc => sc.Code == UnlockCode
                                                                                         && sc.CodeType == SecurityCodeType.UnlockCode
                                                                                         && sc.ExpiryTime.Date == DateTime.Today.AddDays(1)))));
            _communicationService.Verify(s => s.SendAccountLockedMessage(It.Is<User>(c => c.SecurityCodes.Any(sc => sc.Code == UnlockCode
                                                                                                                 && sc.CodeType == SecurityCodeType.UnlockCode)
                                                                                       && c.Id == _user.Id
                                                                                       && c.Email == _user.Email),
                                                                         It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ThenItShouldNotGenerateANewCodeIfTheCurrentCodeHasNotExpired()
        {
            // Arrange
            _user.SecurityCodes = new[]
            {
                new SecurityCode
                {
                    Code = "UNLOCK_CODE",
                    CodeType = SecurityCodeType.UnlockCode,
                    ExpiryTime = DateTime.MaxValue
                }
            };

            // Act
            await _handler.Handle(_event);

            // Assert
            _communicationService.Verify(s => s.SendAccountLockedMessage(It.Is<User>(c => c.SecurityCodes.Any(sc => sc.Code == UnlockCode && sc.CodeType == SecurityCodeType.UnlockCode)
                                                                                       && c.Id == _user.Id
                                                                                       && c.Email == _user.Email), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task ThenItShouldGenerateAnEmailIfTheResendPropertyIsTrue()
        {
            // Arrange
            _event.ResendUnlockCode = true;
            _user.SecurityCodes = new[]
            {
                new SecurityCode
                {
                    Code = "UNLOCK_CODE",
                    CodeType = SecurityCodeType.UnlockCode,
                    ExpiryTime = DateTime.MaxValue
                }
            };

            // Act
            await _handler.Handle(_event);

            // Assert
            _communicationService.Verify(s => s.SendAccountLockedMessage(It.Is<User>(c => c.SecurityCodes.Any(sc => sc.Code == "UNLOCK_CODE" && sc.CodeType == SecurityCodeType.UnlockCode)
                                                                                       && c.Id == _user.Id
                                                                                       && c.Email == _user.Email), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ThenItShouldRegenerateTheCodeIfResendingAndItHasExpired()
        {
            // Arrange
            _event.ResendUnlockCode = true;
            _user.SecurityCodes = new[]
            {
                new SecurityCode
                {
                    Code = "UNLOCK_CODE",
                    CodeType = SecurityCodeType.UnlockCode,
                    ExpiryTime = DateTime.MinValue,
                    ReturnUrl = "http://test.local"
                }
            };

            //Act
            await _handler.Handle(_event);

            // Assert
            _communicationService.Verify(s => s.SendAccountLockedMessage(It.Is<User>(c => c.SecurityCodes.Any(sc => sc.Code == UnlockCode
                                                                                                                 && sc.CodeType == SecurityCodeType.UnlockCode)
                                                                                       && c.Id == _user.Id
                                                                                       && c.Email == _user.Email),
                                                                         It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ThenItShouldUseTheUrlFromTheRequestIfTheUnlockCodeIsNull()
        {
            //Arrange
            _user.SecurityCodes = null;
            _event.ReturnUrl = "http://test.local";

            //Act
            await _handler.Handle(_event);

            //Assert
            _communicationService.Verify(
                s => s.SendAccountLockedMessage(It.Is<User>(c => c.SecurityCodes.Any(sc => sc.ReturnUrl == _event.ReturnUrl)), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenItShouldNotThrowExceptionIfTheUnlockCodeReturnUrlIsNull()
        {
            //Arrange
            _user.SecurityCodes = null;

            //Act
            await _handler.Handle(_event);

            //Assert
            _communicationService.Verify(
                s => s.SendAccountLockedMessage(It.Is<User>(c => c.Email == _user.Email 
                && c.SecurityCodes.Any(sc => sc.ReturnUrl == _employerAccountsBaseUrl)), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenTheUserShouldBeRetrievedByEmailIfTheIdIsNotProvided()
        {
            //Arrange
            _event.User.Id = null;
            _event.User.Email = UserEmail;
            _event.ReturnUrl = "http://test.local";

            //Act
            await _handler.Handle(_event);

            //Assert
            _userRepository.Verify(x => x.GetByEmailAddress(_event.User.Email), Times.Once);
        }

        [Test]
        public async Task ThenItShouldNotCallTheCommunicationServiceIfTheUserDoesNotExist()
        {
            // Arrange
            _event.User.Email = "456789";
            _event.User.Id = string.Empty;

            //Act
            await _handler.Handle(_event);

            // Assert
            _communicationService.Verify(s => s.SendAccountLockedMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Never());
        }
    }
}
