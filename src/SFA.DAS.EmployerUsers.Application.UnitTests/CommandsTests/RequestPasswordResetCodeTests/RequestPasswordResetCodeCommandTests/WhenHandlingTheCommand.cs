﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerUsers.Application.UnitTests.TestHelpers;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;
using SFA.DAS.EmployerUsers.Domain.Links;
using SFA.DAS.TimeProvider;
using SFA.DAS.EmployerUsers.Application.Exceptions;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RequestPasswordResetCodeTests.RequestPasswordResetCodeCommandTests
{
    public class WhenHandlingTheCommand
    {
        private const string ForgottenPasswordLink = "forgotten-credentials";

        private Guid _userId;
        private const string HashedUserId = "123RFVTGB";

        private Mock<IUserRepository> _userRepository;
        private Mock<ICommunicationService> _communicationSerivce;
        private Mock<ICodeGenerator> _codeGenerator;
        private Mock<ILinkBuilder> _linkBuilder;
        private RequestPasswordResetCodeCommandHandler _commandHandler;
        private Mock<ILogger> _logger;
        private Mock<IValidator<RequestPasswordResetCodeCommand>> _validator;
        private Mock<IAuditService> _auditService;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Setup()
        {
            _userId = Guid.NewGuid();
            ConfigurationManager.AppSettings["UseStaticCodeGenerator"] = "false";

            _auditService = new Mock<IAuditService>();
            _userRepository = new Mock<IUserRepository>();
            _communicationSerivce = new Mock<ICommunicationService>();
            _codeGenerator = new Mock<ICodeGenerator>();
            _linkBuilder = new Mock<ILinkBuilder>();
            _logger = new Mock<ILogger>();
            _validator = new Mock<IValidator<RequestPasswordResetCodeCommand>>();
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.HashValue(_userId)).Returns(HashedUserId);

            _linkBuilder.Setup(b => b.GetForgottenPasswordUrl(HashedUserId))
                .Returns(ForgottenPasswordLink);
            
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RequestPasswordResetCodeCommand>())).ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            _commandHandler = new RequestPasswordResetCodeCommandHandler(_validator.Object, 
                _userRepository.Object, _communicationSerivce.Object, _codeGenerator.Object, _linkBuilder.Object, _logger.Object, _auditService.Object, _hashingService.Object);
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public void InvalidCommandThrowsInvalidRequestException()
        {
            _validator.Setup(x => x.ValidateAsync(It.IsAny<RequestPasswordResetCodeCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { {"",""} } });

            var invalidRequestException = Assert.ThrowsAsync<InvalidRequestException>(async () => await _commandHandler.Handle(new RequestPasswordResetCodeCommand()));

            Assert.That(invalidRequestException.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThenItShouldNotSendAnEmailIfNoUserFound()
        {
            //Arrange
            var command = GetRequestPasswordResetCodeCommand();
            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync((User)null);

            //Act
            await _commandHandler.Handle(command);

            //Assert
            _communicationSerivce.Verify(x => x.SendPasswordResetCodeMessage(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task KnownUserWithActiveResetResendsExistingCode()
        {
            var command = GetRequestPasswordResetCodeCommand();

            var code = "ABCDEF";
            var expiryTime = DateTimeProvider.Current.UtcNow.AddHours(1);
            var existingUser = new User
            {
                Id = _userId.ToString(),
                Email = command.Email,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = code,
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = expiryTime
                    }
                }
            };

            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync(existingUser);

            await _commandHandler.Handle(command);

            _communicationSerivce.Verify(x => x.SendPasswordResetCodeMessage(It.Is<User>(u => u.Email == existingUser.Email 
                                                                                              && u.SecurityCodes.Any(sc => sc.Code == code
                                                                                                                           && sc.CodeType == SecurityCodeType.PasswordResetCode
                                                                                                                           && sc.ExpiryTime == expiryTime)
                ), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task KnownUserWithExpiredCodeSendsNewCode()
        {
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.UtcNow);
            const string newCode = "FEDCBA";

            var command = GetRequestPasswordResetCodeCommand();

            var existingUser = new User
            {
                Id = _userId.ToString(),
                Email = command.Email,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "ABCDEF",
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTimeProvider.Current.UtcNow.AddHours(-1)
                    }
                }
            };

            _codeGenerator.Setup(x => x.GenerateAlphaNumeric(6)).Returns(newCode);

            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync(existingUser);

            await _commandHandler.Handle(command);

            _communicationSerivce.Verify(x => x.SendPasswordResetCodeMessage(It.Is<User>(u => u.Email == existingUser.Email 
                                                                                              && u.SecurityCodes.Any(sc => sc.Code == newCode
                                                                                                                           && sc.CodeType == SecurityCodeType.PasswordResetCode
                                                                                                                           && sc.ExpiryTime == DateTimeProvider.Current.UtcNow.AddDays(1))
                ), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCase(2, false, Description = "Less than 3 failed attempts")]
        [TestCase(3, true, Description = "Exactly 3 failed attempts")]
        [TestCase(5, true, Description = "More than 3 failed attempts")]
        public async Task KnownUserWithNonExpiredCode_WithFailedAttemptSendsNewCode(int failedAttempts, bool shouldSendNewCode)
        {
            // Arrange
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.UtcNow);
            const string newCode = "FEDCBA";

            var command = GetRequestPasswordResetCodeCommand();

            var existingUser = new User
            {
                Id = _userId.ToString(),
                Email = command.Email,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "ABCDEF",
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTimeProvider.Current.UtcNow.AddHours(24),
                        FailedAttempts = failedAttempts
                    }
                }
            };

            _codeGenerator.Setup(x => x.GenerateAlphaNumeric(6)).Returns(newCode);
            _userRepository.Setup(x => x.GetByEmailAddress(command.Email)).ReturnsAsync(existingUser);

            // Act 
            await _commandHandler.Handle(command);

            // Assert
            var timesCalled = shouldSendNewCode ? 1 : 0;
            _communicationSerivce.Verify(x => x.SendPasswordResetCodeMessage(It.Is<User>(u => u.Email == existingUser.Email
                                                                                              && u.SecurityCodes.Any(sc => sc.Code == newCode
                                                                                                                           && sc.CodeType == SecurityCodeType.PasswordResetCode
                                                                                                                           && sc.ExpiryTime == DateTimeProvider.Current.UtcNow.AddDays(1))
                ), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(timesCalled));
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