using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Commands.ForcePasswordReset;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Auditing.Update;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ForcePasswordResetTests.ForcePasswordResetCommandHandlerTests
{
    public class WhenHandlingForcePasswordResetCommand
    {
        private const string UserId = "User001";
        private const string SecurityCode = "ABC123";

        private ForcePasswordResetCommandHandler _handler;
        private Mock<IUserRepository> _userRepository;
        private Mock<IAuditService> _auditService;
        private Mock<ICommunicationService> _communicationService;
        private Mock<ICodeGenerator> _codeGenerator;
        private User _existingUser;

        [SetUp]
        public void Arrange()
        {
            _existingUser = new User
            {
                Id = UserId,
                RequiresPasswordReset = false
            };
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetById(UserId))
                .ReturnsAsync(_existingUser);

            _auditService = new Mock<IAuditService>();

            _communicationService = new Mock<ICommunicationService>();

            _codeGenerator = new Mock<ICodeGenerator>();
            _codeGenerator.Setup(g => g.GenerateAlphaNumeric(6))
                .Returns(SecurityCode);

            _handler = new ForcePasswordResetCommandHandler(_userRepository.Object, _auditService.Object, _communicationService.Object, _codeGenerator.Object);
        }

        [Test]
        public async Task ThenItShouldUpdateUserToRequirePasswordReset()
        {
            // Act
            await _handler.Handle(new ForcePasswordResetCommand { UserId = UserId });

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.Id == UserId && u.RequiresPasswordReset)), Times.Once);
        }

        [Test]
        public async Task ThenItShouldUpdateUserWithResetCode()
        {
            // Act
            await _handler.Handle(new ForcePasswordResetCommand { UserId = UserId });

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.Id == UserId && u.SecurityCodes.Any(sc => sc.Code == SecurityCode && sc.CodeType == SecurityCodeType.PasswordResetCode))), Times.Once);
        }

        [Test]
        public async Task ThenItShouldAuditUpdate()
        {
            // Act
            await _handler.Handle(new ForcePasswordResetCommand { UserId = UserId });

            // Assert
            _auditService.Verify(s => s.WriteAudit(It.IsAny<ForcePasswordResetAuditMessage>()), Times.Once);
        }

        [Test]
        public async Task ThenItShouldSendEmailToUser()
        {
            // Act
            await _handler.Handle(new ForcePasswordResetCommand { UserId = UserId });

            // Assert
            _communicationService.Verify(s => s.SendForcePasswordResetMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenItShouldDoNothingIfUserIsAlreadySetToRequirePasswordReset()
        {
            // Arrange
            _existingUser.RequiresPasswordReset = true;

            // Act
            await _handler.Handle(new ForcePasswordResetCommand { UserId = UserId });

            // Assert
            _userRepository.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
            _auditService.Verify(s => s.WriteAudit(It.IsAny<ForcePasswordResetAuditMessage>()), Times.Never);
            _communicationService.Verify(s => s.SendForcePasswordResetMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }
    }
}
