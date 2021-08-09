using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Commands.RequestChangeEmail;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RequestChangeEmailTests.RequestChangeEmailCommandHandlerTests
{
    public class WhenHandling
    {
        private const string UserId = "USER1";
        private const string NewEmailAddress = "user.one@unit.tests";
        private const string OldEmailAddress = "user1@unit.tests";
        private const string ConfirmEmailCode = "AB234B";
        private const string ReturnUrl = "http://unit.test";

        private RequestChangeEmailCommand _command;
        private Mock<IValidator<RequestChangeEmailCommand>> _validator;
        private Mock<IUserRepository> _userRepository;
        private Mock<ICodeGenerator> _codeGenerator;
        private Mock<ICommunicationService> _communicationService;
        private RequestChangeEmailCommandHandler _handler;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void Arrange()
        {
            _command = new RequestChangeEmailCommand
            {
                UserId = UserId,
                NewEmailAddress = NewEmailAddress,
                ConfirmEmailAddress = NewEmailAddress,
                ReturnUrl = ReturnUrl
            };

            _validator = new Mock<IValidator<RequestChangeEmailCommand>>();
            _validator.Setup(v => v.ValidateAsync(_command))
                .ReturnsAsync(new ValidationResult());

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(r => r.GetById(UserId))
                .Returns(Task.FromResult(new User
                {
                    Id = UserId,
                    Email = OldEmailAddress
                }));

            _codeGenerator = new Mock<ICodeGenerator>();
            _codeGenerator.Setup(g => g.GenerateAlphaNumeric(6))
                .Returns(ConfirmEmailCode);

            _communicationService = new Mock<ICommunicationService>();

            _auditService = new Mock<IAuditService>();

            _handler = new RequestChangeEmailCommandHandler(_validator.Object, _userRepository.Object,
                _codeGenerator.Object, _communicationService.Object, _auditService.Object);
        }

        [Test]
        public void ThenItShouldThrowAnInvalidRequestExceptionIfCommandNotValid()
        {
            // Arrange
            var errors = new Dictionary<string, string> { { "", "" } };
            _validator.Setup(v => v.ValidateAsync(_command))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = errors });

            // Act + Assert
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));
            Assert.AreSame(errors, actual.ErrorMessages);
        }

        [Test]
        public void ThenItShouldThrowAnInvalidRequestExceptionIfUserNotFound()
        {
            // Arrange
            _userRepository.Setup(r => r.GetById(UserId))
                .Returns(Task.FromResult<User>(null));

            // Act + Assert
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));
            Assert.IsTrue(actual.ErrorMessages.ContainsKey(""));
            Assert.AreEqual("Cannot find user", actual.ErrorMessages[""]);
        }

        [Test]
        public async Task ThenItShouldUpdateTheUserWithANewConfirmEmailSecurityCode()
        {
            // Act
            await _handler.Handle(_command);

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.Id == UserId
                                                               && u.SecurityCodes.Any(sc => sc.Code == ConfirmEmailCode
                                                                                         && sc.CodeType == SecurityCodeType.ConfirmEmailCode
                                                                                         && sc.ReturnUrl == ReturnUrl
                                                                                         && sc.PendingValue == NewEmailAddress))
                                                ), Times.Once);
        }

        [Test]
        public async Task ThenitShouldSendTheUserAnEmailWithTheCodeToTheirNewAddress()
        {
            // Act
            await _handler.Handle(_command);

            // Assert
            _communicationService.Verify(s => s.SendConfirmEmailChangeMessage(It.Is<User>(u => u.Id == UserId), It.IsAny<string>()));
        }

    }
}
