using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.PasswordResetTests.ValidatePasswordResetCodeCommandTests
{
    public class WhenHandlingTheCommand
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IValidator<ValidatePasswordResetCodeCommand>> _validator;
        private ValidatePasswordResetCodeCommandHandler _sut;
        private const string ActualEmailAddress = "someuser@local";
        public string PasswordResetCode = "123456ABC";
        private Mock<ILogger> _logger;
        

        [SetUp]
        public void Arrange()
        {
            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _userRepository.Setup(x => x.GetByEmailAddress(ActualEmailAddress))
                .ReturnsAsync(new User
                {
                    Email = ActualEmailAddress,
                    IsActive = true,
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = PasswordResetCode,
                            CodeType = SecurityCodeType.PasswordResetCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                });

            _validator = new Mock<IValidator<ValidatePasswordResetCodeCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<ValidatePasswordResetCodeCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _logger = new Mock<ILogger>();

            _sut = new ValidatePasswordResetCodeCommandHandler(
                _userRepository.Object, 
                _validator.Object, 
                _logger.Object);
        }

        [Test]
        public async Task ThenTheUserIsReturnedFromTheRespository()
        {
            // Act
            await _sut.Handle(new ValidatePasswordResetCodeCommand { Email = ActualEmailAddress });

            // Assert
            _userRepository.Verify(x => x.GetByEmailAddress(ActualEmailAddress));

        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            // Act
            await _sut.Handle(new ValidatePasswordResetCodeCommand { Email = ActualEmailAddress });

            // Assert
            _validator.Verify(x => x.ValidateAsync(It.IsAny<ValidatePasswordResetCodeCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheMessageIsPopulatedwithUserAndValidated()
        {
            //Act
            await _sut.Handle(new ValidatePasswordResetCodeCommand { Email = ActualEmailAddress });

            //Assert
            _validator.Verify(x => x.ValidateAsync(It.Is<ValidatePasswordResetCodeCommand>(c => c.User != null)), Times.Once);
        }

        [Test]
        public void ThenAInvalidRequestExceptionIsThrownIfTheMessageIsNotValid()
        {
            // Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<ValidatePasswordResetCodeCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            // Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _sut.Handle(new ValidatePasswordResetCodeCommand()));

            // Assert
            _userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void ThenTheFailedAttemptsAreIncrementedIfTheResetCodeIsInvalid()
        {
            // Arrange
            var userEmail = "someotheremail@local";
            _userRepository.Setup(x => x.GetByEmailAddress(It.Is<string>(s => s == userEmail))).ReturnsAsync(new User
            {
                Id = "USER1",
                Email = ActualEmailAddress,
                IsActive = true,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "143XYZ",
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTime.MaxValue,
                        FailedAttempts = 0
                    },
                }
            });

            _validator.Setup(x => x.ValidateAsync(It.IsAny<ValidatePasswordResetCodeCommand>())).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            // Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _sut.Handle(new ValidatePasswordResetCodeCommand { Email = userEmail }));

            // Assert
            _userRepository.Verify(x => x.Update(It.Is<User>(u => u.SecurityCodes[0].FailedAttempts == 1)), Times.Once);
        }

        [Test]
        public void ThenAnExceededLimitPasswordResetCodeExceptionIsThrownIfTooManyInvalidAttemptHaveBeenMade()
        {
            // Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<ValidatePasswordResetCodeCommand>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>())).ReturnsAsync(new User
            {
                Id = "USER1",
                Email = ActualEmailAddress,
                IsActive = true,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "123456",
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTime.MaxValue,
                        FailedAttempts = 2
                    },
                }
            });

            // Act
            Assert.ThrowsAsync<ExceededLimitPasswordResetCodeException>(async () => await _sut.Handle(new ValidatePasswordResetCodeCommand()));

            // Assert
            _userRepository.Verify(x => x.Update(It.Is<User>(u => u.SecurityCodes[0].FailedAttempts == 3)), Times.Once);
        }
    }
}
