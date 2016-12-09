using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ChangeEmail;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ChangeEmailTests.ChangeEmailCommandValidatorTests
{
    public class WhenValidating
    {
        private const string SecurityCode = "1A2B3C0";
        private const string Password = "password";
        private const string Hashedpassword = "HASHED" + Password;
        private const string Salt = "SALT";
        private const string PasswordProfileId = "PROFILE1";

        private Mock<IPasswordService> _passwordService;
        private ChangeEmailCommandValidator _validator;
        private ChangeEmailCommand _command;

        [SetUp]
        public void Arrange()
        {
            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(s => s.VerifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            _passwordService.Setup(s => s.VerifyAsync(Password, Hashedpassword, Salt, PasswordProfileId))
                .Returns(Task.FromResult(true));

            _validator = new ChangeEmailCommandValidator(_passwordService.Object);

            _command = new ChangeEmailCommand
            {
                User = new Domain.User
                {
                    Id = "USER1",
                    Password = Hashedpassword,
                    Salt = Salt,
                    PasswordProfileId = PasswordProfileId,
                    SecurityCodes = new[]
                    {
                        new Domain.SecurityCode
                        {
                            Code = SecurityCode,
                            CodeType = Domain.SecurityCodeType.ConfirmEmailCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                },
                SecurityCode = SecurityCode,
                Password = Password
            };
        }

        [Test]
        public async Task ThenItShouldAValidResultIfNoErrors()
        {
            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenItShouldReturnAnErrorIfTheUserIsNotSpecified()
        {
            // Arrange
            _command.User = null;

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertResultContainsError(actual, "User", "User Does Not Exist");
        }

        [Test]
        public async Task ThenItShouldReturnAnErrorIfTheSecurityCodeIsNotSpecified()
        {
            // Arrange
            _command.SecurityCode = null;

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertResultContainsError(actual, "SecurityCode", "Security code has not been provided");
        }

        [Test]
        public async Task ThenItShouldReturnAnErrorIfThePasswordIsNotSpecified()
        {
            // Arrange
            _command.Password = null;

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertResultContainsError(actual, "Password", "Password has not been provided");
        }

        [Test]
        public async Task ThenItShouldReturnAnErrorIfThePasswordDoesNotMatchUsers()
        {
            // Arrange
            _command.Password = Password + "_INVALID";

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertResultContainsError(actual, "Password", "Password is incorrect");
        }

        [Test]
        public async Task ThenItShouldReturnAnErrorIfTheSecurityCodeDoesNotExistForTheUser()
        {
            // Arrange
            _command.SecurityCode = SecurityCode + "_INVALID";

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertResultContainsError(actual, "SecurityCode", "Security code is incorrect");
        }

        [Test]
        public async Task ThenItShouldReturnAnErrorIfTheSecurityCodeExistsButHasExpired()
        {
            // Arrange
            _command.User.SecurityCodes[0].ExpiryTime = DateTime.UtcNow.AddMinutes(-1);

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertResultContainsError(actual, "SecurityCode", "Security code has expired");
        }

        [Test]
        public async Task ThenItShouldCompareSecurityCodesByIgnoringCase()
        {
            // Arrange
            _command.SecurityCode = SecurityCode.ToInverseCase();

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }



        private void AssertResultContainsError(ValidationResult result, string key, string message)
        {
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid());
            Assert.IsNotNull(result.ValidationDictionary);
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(key));
            Assert.AreEqual(message, result.ValidationDictionary[key]);
        }
    }
}
