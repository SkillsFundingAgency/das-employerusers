using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ActivateUserTests.ActivateUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private ActivateUserCommandHandler _activateUserCommand;
        private Mock<IValidator<ActivateUserCommand>> _activateUserCommandValidator;
        private Mock<IUserRepository> _userRepository;
        private Mock<ICommunicationService> _communicationSerivce;

        [SetUp]
        public void Arrange()
        {
            _activateUserCommandValidator = new Mock<IValidator<ActivateUserCommand>>();

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(new Domain.User());
            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>())).ReturnsAsync(new Domain.User());

            _communicationSerivce = new Mock<ICommunicationService>();

            _activateUserCommand = new ActivateUserCommandHandler(_activateUserCommandValidator.Object, _userRepository.Object, _communicationSerivce.Object);
        }

        [Test]
        public async Task ThenActivateUserCommandValidatorIsCalled()
        {
            //Arrange
            var accessCode = "123DSAD";
            var userId = Guid.NewGuid().ToString();
            var activateUserCommand = new ActivateUserCommand { AccessCode = accessCode, UserId = userId };
            _activateUserCommandValidator.Setup(x => x.Validate(activateUserCommand)).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _activateUserCommand.Handle(activateUserCommand);

            //Assert
            _activateUserCommandValidator.Verify(x => x.Validate(It.Is<ActivateUserCommand>(c => c.UserId == userId && c.AccessCode == accessCode)), Times.Once);
        }

        [Test]
        public async Task ThenTheUserRepositoryIsCalledIfTheCommandIsValid()
        {
            //Arrange
            _activateUserCommandValidator.Setup(x => x.Validate(It.IsAny<ActivateUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _activateUserCommand.Handle(new ActivateUserCommand());

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<Domain.User>()), Times.Once);

        }


        [Test]
        public void ThenTheUserRepositoryIsNotCalledIfTheCommandIsInValid()
        {
            //Arrange
            _activateUserCommandValidator.Setup(x => x.Validate(It.IsAny<ActivateUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _activateUserCommand.Handle(new ActivateUserCommand()));

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<Domain.User>()), Times.Never);
        }


        [Test]
        public void ThenAInvalidDataExceptionIsThrownIfTheCommandIsInValid()
        {
            //Arrange
            _activateUserCommandValidator.Setup(x => x.Validate(It.IsAny<ActivateUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _activateUserCommand.Handle(new ActivateUserCommand()));

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<Domain.User>()), Times.Never);
        }


        [Test]
        public async Task ThenTheUserIsRetrievedFromTheUserRepositoryIfTheCommandIsValid()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            _activateUserCommandValidator.Setup(x => x.Validate(It.IsAny<ActivateUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _activateUserCommand.Handle(new ActivateUserCommand { UserId = userId });

            //Assert
            _userRepository.Verify(x => x.GetById(userId), Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsRetrievedByEmailIfTheUserIdIsNotPresent()
        {
            //Arrange
            var emailAddress = "test@local.com";
            _activateUserCommandValidator.Setup(x => x.Validate(It.IsAny<ActivateUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _activateUserCommand.Handle(new ActivateUserCommand { Email = emailAddress });

            //Assert
            _userRepository.Verify(x => x.GetByEmailAddress(emailAddress), Times.Once);
        }

        [Test]
        public async Task ThenThenUserIsUpdatedWithTheCorrectDetails()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var accessCode = "123ADF&^%";
            var user = new Domain.User
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
            _activateUserCommandValidator.Setup(x => x.Validate(It.Is<ActivateUserCommand>(p => p.AccessCode == accessCode && p.UserId == userId))).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _activateUserCommand.Handle(activateUserCommand);

            //Assert
            _userRepository.Verify(x => x.Update(It.Is<Domain.User>(p => p.IsActive && p.Id == userId)), Times.Once);
        }

        [Test]
        public async Task ThenTheUsersAccessCodesAreExpired()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var accessCode = "123ADF&^%";
            var user = new Domain.User
            {
                Email = "test@test.com",
                LastName = "Tester",
                FirstName = "Test",
                Password = "SomePassword",
                IsActive = false,
                Id = userId,
                SecurityCodes = new[]
                {
                    new Domain.SecurityCode
                    {
                        Code = "123456",
                        CodeType = Domain.SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new Domain.SecurityCode
                    {
                        Code = "987654",
                        CodeType = Domain.SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new Domain.SecurityCode
                    {
                        Code = "852149",
                        CodeType = Domain.SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new Domain.SecurityCode
                    {
                        Code = "785236",
                        CodeType = Domain.SecurityCodeType.UnlockCode,
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
            _activateUserCommandValidator.Setup(x => x.Validate(It.Is<ActivateUserCommand>(p => p.AccessCode == accessCode && p.UserId == userId))).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _activateUserCommand.Handle(activateUserCommand);

            //Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.Id == userId
                                                               && u.SecurityCodes.Length == 2
                                                               && !u.SecurityCodes.Any(sc => sc.CodeType == SecurityCodeType.AccessCode))),
                                   Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsEmailedAboutThereAccountCreation()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            _activateUserCommandValidator.Setup(x => x.Validate(It.IsAny<ActivateUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _activateUserCommand.Handle(new ActivateUserCommand { UserId = userId });

            //Assert
            _communicationSerivce.Verify(x => x.SendUserAccountConfirmationMessage(It.IsAny<Domain.User>(), It.IsAny<string>()), Times.Once);
        }


        [Test]
        public void ThenTheUserIsNotEmailedAboutThereAccountCreationIfItFailsValidation()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            _activateUserCommandValidator.Setup(x => x.Validate(It.IsAny<ActivateUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _activateUserCommand.Handle(new ActivateUserCommand()));

            //Assert
            _communicationSerivce.Verify(x => x.SendUserAccountConfirmationMessage(It.IsAny<Domain.User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheUserIsNotEmailedAndTheUserIsNotUpdatedIfTheUserIsAlreadyActive()
        {
            //Arrange
            var userId = Guid.NewGuid().ToString();
            var accessCode = "123ADF&^%";
            var user = new Domain.User
            {
                Email = "test@test.com",
                LastName = "Tester",
                FirstName = "Test",
                Password = "SomePassword",
                IsActive = true,
                Id = userId,
                AccessCode = accessCode
            };
            var activateUserCommand = new ActivateUserCommand
            {
                UserId = userId,
                AccessCode = accessCode
            };
            _userRepository.Setup(x => x.GetById(userId)).ReturnsAsync(user);
            _activateUserCommandValidator.Setup(x => x.Validate(It.Is<ActivateUserCommand>(p => p.AccessCode == accessCode && p.UserId == userId && p.User.AccessCode == accessCode))).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _activateUserCommand.Handle(activateUserCommand);

            //Assert
            _userRepository.Verify(x => x.Update(It.Is<Domain.User>(p => p.IsActive && p.Id == userId)), Times.Never);
            _communicationSerivce.Verify(x => x.SendUserAccountConfirmationMessage(It.IsAny<Domain.User>(), It.IsAny<string>()), Times.Never);
        }
    }
}
