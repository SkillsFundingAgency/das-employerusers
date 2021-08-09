using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ResendActivationCodeTests.ResendActivationCodeCommandTests
{
    [TestFixture]
    public class WhenHandlingTheCommand
    {
        private Mock<IValidator<ResendActivationCodeCommand>> _validator;
        private Mock<IUserRepository> _userRepository;
        private Mock<ICommunicationService> _communicationSerivce;
        private ResendActivationCodeCommandHandler _commandHandler;
        private Mock<ILogger> _logger;
        private Mock<ICodeGenerator> _codeGenerator;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void Setup()
        {
            _validator = new Mock<IValidator<ResendActivationCodeCommand>>();
            _userRepository = new Mock<IUserRepository>();
            _communicationSerivce = new Mock<ICommunicationService>();
            _codeGenerator = new Mock<ICodeGenerator>();
            _logger = new Mock<ILogger>();
            _auditService = new Mock<IAuditService>();

            _commandHandler = new ResendActivationCodeCommandHandler(
                _validator.Object, 
                _userRepository.Object, 
                _communicationSerivce.Object, 
                _codeGenerator.Object, 
                _logger.Object, 
                _auditService.Object);
        }

        [Test]
        public void ThenThrowsExceptionIfCommandFailsValidation()
        {
            SetValidatorToReturn(false);

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _commandHandler.Handle(new ResendActivationCodeCommand()));
        }

        [Test]
        public void ThenThrowsExceptionIfUserNotFound()
        {
            SetValidatorToReturn(true);
            SetUserRepositoryToReturn(null);

            Assert.ThrowsAsync<InvalidRequestException>(async () => await _commandHandler.Handle(new ResendActivationCodeCommand()));
        }

        [Test]
        public async Task ThenShouldCallResendIfNotActiveUserFound()
        {
            SetValidatorToReturn(true);
            SetUserRepositoryToReturn(new User
            {
                IsActive = false,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "CODE1",
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    }
                }
            });

            await _commandHandler.Handle(new ResendActivationCodeCommand());

            _communicationSerivce.Verify(x => x.ResendActivationCodeMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenShouldNotCallResendIfActiveUserFound()
        {
            SetValidatorToReturn(true);
            SetUserRepositoryToReturn(new User
            {
                IsActive = true
            });

            await _commandHandler.Handle(new ResendActivationCodeCommand());

            _communicationSerivce.Verify(x => x.ResendActivationCodeMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenShouldGenerateNewAccessCodeIfPreviousHasExpired()
        {
            // Arrange
            SetValidatorToReturn(true);
            SetUserRepositoryToReturn(new User
            {
                IsActive = false,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "CODE1",
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.Today.AddSeconds(-1)
                    }
                }
            });

            // Act
            await _commandHandler.Handle(new ResendActivationCodeCommand());

            // Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.SecurityCodes.Any(sc => sc.CodeType == SecurityCodeType.AccessCode && sc.ExpiryTime >= DateTime.Now))), Times.Once);
        }

        private void SetValidatorToReturn(bool isValid)
        {
            if (isValid)
            {
                _validator.Setup(x => x.ValidateAsync(It.IsAny<ResendActivationCodeCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            }
            else
            {
                _validator.Setup(x => x.ValidateAsync(It.IsAny<ResendActivationCodeCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });
            }
        }

        private void SetUserRepositoryToReturn(User user)
        {
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(user);
        }
    }
}