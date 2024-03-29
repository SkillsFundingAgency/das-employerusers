﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Auditing;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RegisterUserTests.RegisterUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private const string ReturnUrl = "http://unit.test";
        private RegisterUserCommandHandler _registerUserCommandHandler;
        private Mock<IValidator<RegisterUserCommand>> _registerUserCommandValidator;
        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordService> _passwordService;
        private Mock<ICommunicationService> _communicationService;
        private Mock<ICodeGenerator> _codeGenerator;
        private Mock<ILogger> _logger;
        private Mock<IAuditService> _auditService;

        [SetUp]
        public void Arrange()
        {
            _registerUserCommandValidator = new Mock<IValidator<RegisterUserCommand>>();
            _auditService = new Mock<IAuditService>();

            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(s => s.GenerateAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new SecuredPassword
                {
                    HashedPassword = "Secured_Password",
                    Salt = "Generated_Salt",
                    ProfileId = "Password_Profile_Id"
                }));

            _userRepository = new Mock<IUserRepository>();

            _communicationService = new Mock<ICommunicationService>();

            _codeGenerator = new Mock<ICodeGenerator>();
            _codeGenerator.Setup(x => x.GenerateAlphaNumeric(6))
                .Returns("ABC123XYZ");

            _auditService = new Mock<IAuditService>();

            _logger = new Mock<ILogger>();

            _registerUserCommandHandler = new RegisterUserCommandHandler(_registerUserCommandValidator.Object,
                                                                         _passwordService.Object,
                                                                         _userRepository.Object,
                                                                         _communicationService.Object,
                                                                         _codeGenerator.Object,
                                                                         _logger.Object,
                                                                         _auditService.Object);
        }

        [Test]
        public void ThenTheCommandIsHandledAndValidatorCalled()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "MyError", "Some error has happened" } } });

            //Act
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));

            //Assert
            _registerUserCommandValidator.Verify(x => x.ValidateAsync(It.IsAny<RegisterUserCommand>()));
            Assert.Contains(new KeyValuePair<string, string>("MyError", "Some error has happened"), actual.ErrorMessages);
        }

        [Test]
        public async Task ThenTheUserIsCreatedIfTheCommandIsValid()
        {
            //Arrange
            var firstName = "Test";
            var lastName = "Tester";
            var emailAddress = "test@test.com";
            var password = "password";
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = emailAddress,
                Password = password,
                ConfirmPassword = password,
                ReturnUrl = ReturnUrl
            };
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(registerUserCommand)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _registerUserCommandHandler.Handle(registerUserCommand);

            //Assert
            _userRepository.Verify(v => v.Create(It.Is<User>(x => x.FirstName.Equals(firstName)
                                                               && x.LastName.Equals(lastName)
                                                               && x.Email.Equals(emailAddress)
                                                               && !x.Password.Equals(password)
                                                               && x.SecurityCodes.Any(sc => sc.Code == "ABC123XYZ"
                                                                                         && sc.CodeType == SecurityCodeType.AccessCode
                                                                                         && sc.ReturnUrl == ReturnUrl))));
        }

        [Test]

        public void ThenTheUserIsNotCreatedIfTheCommandIsInvalid()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));

            //Assert
            _userRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void ThenAnExceptionIsThrownIfTheCommandIsInvalid()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });


            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));
        }

        [Test]
        public async Task ThenTheUserIsCreatedWithSecurePasswordDetails()
        {
            // Arrange
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = "Unit",
                LastName = "Tests",
                Email = "unit.tests@test.local",
                Password = "SomePassword",
                ConfirmPassword = "SomePassword"
            };
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(registerUserCommand)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            // Act
            await _registerUserCommandHandler.Handle(registerUserCommand);

            // Assert
            _userRepository.Verify(r => r.Create(It.Is<User>(u => u.Password == "Secured_Password"
                                                               && u.Salt == "Generated_Salt"
                                                               && u.PasswordProfileId == "Password_Profile_Id"
                                                               && u.SecurityCodes.Any(sc => sc.Code == "ABC123XYZ"
                                                                                         && sc.CodeType == SecurityCodeType.AccessCode
                                                                                         && sc.ExpiryTime == DateTime.Today.AddDays(8).AddSeconds(-1)))));
        }

        [Test]
        public async Task ThenTheUserIsCreatedWithSecurePasswordDetailsInHistory()
        {
            // Arrange
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = "Unit",
                LastName = "Tests",
                Email = "unit.tests@test.local",
                Password = "SomePassword",
                ConfirmPassword = "SomePassword"
            };
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(registerUserCommand)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            // Act
            await _registerUserCommandHandler.Handle(registerUserCommand);

            // Assert
            _userRepository.Verify(r => r.Create(It.Is<User>(u => u.PasswordHistory != null
                                                               && u.PasswordHistory.Length == 1
                                                               && u.PasswordHistory[0].Password == "Secured_Password"
                                                               && u.PasswordHistory[0].PasswordProfileId == "Password_Profile_Id"
                                                               && u.PasswordHistory[0].Salt == "Generated_Salt")));
        }

        [Test]
        [Ignore("Can have intermittent issues due to fire and foreget notification")]
        public async Task ThenTheCommunicationServiceIsCalledOnSuccessfulCommand()
        {
            // Arrange
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = "Unit",
                LastName = "Tests",
                Email = "unit.tests@test.local",
                Password = "SomePassword",
                ConfirmPassword = "SomePassword"
            };
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(registerUserCommand)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _registerUserCommandHandler.Handle(registerUserCommand);

            //Assert
            _communicationService.Verify(x => x.SendUserRegistrationMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        [Ignore("Can have intermittent issues due to fire and foreget notification")]
        public async Task ThenTheAccessCodeIsSentToTheUserOnSuccessfulCreation()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = "Unit",
                LastName = "Tests",
                Email = "unit.tests@test.local",
                Password = "SomePassword",
                ConfirmPassword = "SomePassword",
                Id = userId
            };
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(registerUserCommand)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _registerUserCommandHandler.Handle(registerUserCommand);

            //Assert
            _communicationService.Verify(x => x.SendUserRegistrationMessage(It.Is<User>(s => s.SecurityCodes.Any(sc => sc.Code == "ABC123XYZ"
                                                                                                                    && sc.CodeType == SecurityCodeType.AccessCode)
                                                                                         && s.Id.Equals(userId)), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ThenTheAccessCodeIsNotSentToTheUserOnUnSuccessfulCreation()
        {
            // Arrange
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));

            //Assert
            _communicationService.Verify(x => x.SendUserRegistrationMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheAccessCodeIsProvidedFromTheCodeGenerator()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = "Unit",
                LastName = "Tests",
                Email = "unit.tests@test.local",
                Password = "SomePassword",
                ConfirmPassword = "SomePassword",
                Id = userId
            };
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(registerUserCommand)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _registerUserCommandHandler.Handle(registerUserCommand);

            //Assert
            _codeGenerator.Verify(x => x.GenerateAlphaNumeric(6), Times.Once);
        }

        [Test]
        public void ThenTheEmailAddressIsAlreadyInUse()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var registerUserCommand = new RegisterUserCommand
            {
                Email = "unit.tests@test.local",
                Id = userId
            };
            _registerUserCommandValidator.Setup(x => x.ValidateAsync(registerUserCommand)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _userRepository.Setup(x => x.GetByEmailAddress(registerUserCommand.Email)).ReturnsAsync(new User
            {
                Email = registerUserCommand.Email,
                IsActive = true
            });

            // Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(registerUserCommand));
        }
    }
}
