﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ActivateUserTests.ActivateUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private const string AccessCode = "ABC123";
        private const string ReturnUrl = "http://unit.test";
        private const string UserId = "USER1";

        private User _user;
        private ActivateUserCommandHandler _handler;
        private Mock<IValidator<ActivateUserCommand>> _activateUserCommandValidator;
        private Mock<IUserRepository> _userRepository;
        private ActivateUserCommand _command;
        private Mock<ILogger> _logger;
        private Mock<IAuditService> _auditService;


        [SetUp]
        public void Arrange()
        {
            _activateUserCommandValidator = new Mock<IValidator<ActivateUserCommand>>();
            _activateUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<ActivateUserCommand>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _user = new User
            {
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = AccessCode,
                        CodeType = SecurityCodeType.AccessCode,
                        ReturnUrl = ReturnUrl
                    }
                }
            };
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>())).ReturnsAsync(_user);

            _auditService = new Mock<IAuditService>();

            _logger = new Mock<ILogger>();

            _handler = new ActivateUserCommandHandler(_activateUserCommandValidator.Object, _userRepository.Object, _auditService.Object, _logger.Object);

            _command = new ActivateUserCommand
            {
                AccessCode = AccessCode,
                UserId = UserId
            };
        }

        [Test]
        public async Task ThenActivateUserCommandValidatorIsCalled()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _activateUserCommandValidator.Verify(x => x.ValidateAsync(It.Is<ActivateUserCommand>(c => c.UserId == UserId && c.AccessCode == AccessCode)), Times.Once);
        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledIfTheCommandIsValid()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);

        }


        [Test]
        public void ThenTheUserRepositoryIsNotCalledIfTheCommandIsInValid()
        {
            //Arrange
            _activateUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<ActivateUserCommand>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }


        [Test]
        public void ThenAInvalidDataExceptionIsThrownIfTheCommandIsInValid()
        {
            //Arrange
            _activateUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<ActivateUserCommand>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }


        [Test]
        public async Task ThenTheUserIsRetrievedFromTheUserRepositoryIfTheCommandIsValid()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _userRepository.Verify(x => x.GetById(UserId), Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsRetrievedByEmailIfTheUserIdIsNotPresent()
        {
            //Arrange
            var emailAddress = "test@local.com";

            //Act
            await _handler.Handle(new ActivateUserCommand { Email = emailAddress });

            //Assert
            _userRepository.Verify(x => x.GetByEmailAddress(emailAddress), Times.Once);
        }

        [Test]
        public async Task ThenThenUserIsUpdatedWithTheCorrectDetails()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var accessCode = "123ADF&^%";
            var user = new User
            {
                Email = "test@test.com",
                LastName = "Tester",
                FirstName = "Test",
                Password = "SomePassword",
                IsActive = false,
                Id = userId
            };
            var activateUserCommand = new ActivateUserCommand
            {
                UserId = userId,
                AccessCode = accessCode
            };
            _userRepository.Setup(x => x.GetById(userId)).ReturnsAsync(user);
            _activateUserCommandValidator.Setup(x => x.ValidateAsync(It.Is<ActivateUserCommand>(p => p.AccessCode == accessCode && p.UserId == userId))).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _handler.Handle(activateUserCommand);

            //Assert
            _userRepository.Verify(x => x.Update(It.Is<User>(p => p.IsActive && p.Id == userId)), Times.Once);
        }

        [Test]
        public async Task ThenTheUsersAccessCodesAreExpired()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var accessCode = "123ADF&^%";
            var user = new User
            {
                Email = "test@test.com",
                LastName = "Tester",
                FirstName = "Test",
                Password = "SomePassword",
                IsActive = false,
                Id = userId,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "123456",
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "987654",
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "852149",
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "785236",
                        CodeType = SecurityCodeType.UnlockCode,
                        ExpiryTime = DateTime.MaxValue
                    }
                }
            };
            var activateUserCommand = new ActivateUserCommand
            {
                UserId = userId,
                AccessCode = accessCode
            };
            _userRepository.Setup(x => x.GetById(userId)).ReturnsAsync(user);
            _activateUserCommandValidator.Setup(x => x.ValidateAsync(It.Is<ActivateUserCommand>(p => p.AccessCode == accessCode && p.UserId == userId))).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _handler.Handle(activateUserCommand);

            //Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.Id == userId
                                                               && u.SecurityCodes.Length == 2
                                                               && !u.SecurityCodes.Any(sc => sc.CodeType == SecurityCodeType.AccessCode))),
                                   Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsNotUpdatedIfTheUserIsAlreadyActive()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var accessCode = "123ADF&^%";
            var user = new User
            {
                Email = "test@test.com",
                LastName = "Tester",
                FirstName = "Test",
                Password = "SomePassword",
                IsActive = true,
                Id = userId,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = accessCode,
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    }
                }
            };
            var activateUserCommand = new ActivateUserCommand
            {
                UserId = userId,
                AccessCode = accessCode
            };
            _userRepository.Setup(x => x.GetById(userId)).ReturnsAsync(user);
            _activateUserCommandValidator.Setup(x => x.ValidateAsync(It.Is<ActivateUserCommand>(p => p.AccessCode == accessCode
                                                                                             && p.UserId == userId
                                                                                             && p.User.SecurityCodes.Any(sc => sc.Code == accessCode
                                                                                                                            && sc.CodeType == SecurityCodeType.AccessCode))))
                                         .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _handler.Handle(activateUserCommand);

            //Assert
            _userRepository.Verify(x => x.Update(It.Is<User>(p => p.IsActive && p.Id == userId)), Times.Never);
        }

        [TestCase("ABC123")]
        [TestCase("abc123")]
        public async Task ThenTheSecurityCodeReturnUrlIsReturned(string enteredAccessCode)
        {
            // Arrange
            _command.AccessCode = enteredAccessCode;

            // Act
            var actual = await _handler.Handle(_command);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(ReturnUrl, actual.ReturnUrl);
        }
    }
}
