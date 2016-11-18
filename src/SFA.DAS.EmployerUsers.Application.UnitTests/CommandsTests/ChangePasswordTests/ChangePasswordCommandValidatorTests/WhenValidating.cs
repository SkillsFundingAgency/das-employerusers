using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ChangePassword;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ChangePasswordTests.ChangePasswordCommandValidatorTests
{
    public class WhenValidating
    {
        private const string UserId = "USER1";
        private const string CurrentPasswordHash = "HASHEDPASSWORD";
        private const string CurrentPassword = "password";
        private const string PasswordProfileId = "PASSWORDPROFILE";
        private const string Salt = "SALTY";
        private const string NewPassword = "Password1";

        private Mock<IPasswordService> _passwordService;
        private ChangePasswordCommandValidator _validator;
        private ChangePasswordCommand _command;

        [SetUp]
        public void Arrange()
        {
            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(s => s.VerifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            _passwordService.Setup(s => s.VerifyAsync(CurrentPassword, CurrentPasswordHash, Salt, PasswordProfileId))
                .Returns(Task.FromResult(true));

            _validator = new ChangePasswordCommandValidator(_passwordService.Object);

            _command = new ChangePasswordCommand
            {
                User = new Domain.User
                {
                    Id = UserId,
                    Password = CurrentPasswordHash,
                    PasswordProfileId = PasswordProfileId,
                    Salt = Salt
                },
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = NewPassword
            };
        }

        [Test]
        public void ThenItShouldReturnAValidResultIfNoProblems()
        {
            // Act
            var actual = _validator.Validate(_command);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenItShouldReturnAnInvalidResultWithErrorIfCurrentPasswordDoesNotMatchUser()
        {
            // Arrange
            _command.CurrentPassword = CurrentPassword + "_invalid";

            // Act
            var actual = _validator.Validate(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "CurrentPassword", "Invalid password");
        }

        [Test]
        public void ThenItShouldReturnAnInvalidResultWithErrorIfConfirmPasswordDoesNotMatchNewPassword()
        {
            // Arrange
            _command.ConfirmPassword = NewPassword + "_invalid";

            // Act
            var actual = _validator.Validate(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "ConfirmPassword", "Passwords do not match");
        }

        [Test]
        public void ThenItShouldReturnAnInvalidResultWithErrorIfNewPasswordIsNotBetween8And16Characters()
        {
            // Arrange
            _command.NewPassword = "1234567";

            // Act
            var actual = _validator.Validate(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "NewPassword", "Password does not meet requirements");
        }

        [TestCase("abcdefghijk1")]
        [TestCase("ABCDEFGHIJK1")]
        public void ThenItShouldReturnAnInvalidResultWithErrorIfNewPasswordDoesNotHave1UpperAnd1Lower(string password)
        {
            // Arrange
            _command.NewPassword = password;

            // Act
            var actual = _validator.Validate(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "NewPassword", "Password does not meet requirements");
        }

        [Test]
        public void ThenItShouldReturnAnInvalidResultWithErrorIfNewPasswordDoesNotContainAtLeast1Number()
        {
            // Arrange
            _command.NewPassword = "abcdefghijk";

            // Act
            var actual = _validator.Validate(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "NewPassword", "Password does not meet requirements");
        }



        private void AssertExpectedValidationErrorExists(ValidationResult result, string key, string message)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ValidationDictionary);
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(key));
            Assert.AreEqual(message, result.ValidationDictionary[key]);
        }
    }
}
