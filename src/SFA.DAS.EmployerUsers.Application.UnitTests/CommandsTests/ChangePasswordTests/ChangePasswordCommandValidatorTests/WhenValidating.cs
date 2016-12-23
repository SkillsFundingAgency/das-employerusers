using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Configuration;
using SFA.DAS.EmployerUsers.Application.Commands.ChangePassword;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Infrastructure.Configuration;

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
        private Mock<IConfigurationService> _configurationService;
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

            _configurationService = new Mock<IConfigurationService>();
            _configurationService.Setup(s => s.GetAsync<EmployerUsersConfiguration>())
                .ReturnsAsync(new EmployerUsersConfiguration
                {
                    Account = new AccountConfiguration
                    {
                        NumberOfPasswordsInHistory = 3
                    }
                });

            _validator = new ChangePasswordCommandValidator(_passwordService.Object, _configurationService.Object);

            _command = new ChangePasswordCommand
            {
                User = new Domain.User
                {
                    Id = UserId,
                    Password = CurrentPasswordHash,
                    PasswordProfileId = PasswordProfileId,
                    Salt = Salt,
                    PasswordHistory = new Domain.HistoricalPassword[0]
                },
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = NewPassword,
            };
        }

        [Test]
        public async Task ThenItShouldReturnAValidResultIfNoProblems()
        {
            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResultWithErrorIfCurrentPasswordDoesNotMatchUser()
        {
            // Arrange
            _command.CurrentPassword = CurrentPassword + "_invalid";

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "CurrentPassword", "Invalid password");
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResultWithErrorIfConfirmPasswordDoesNotMatchNewPassword()
        {
            // Arrange
            _command.ConfirmPassword = NewPassword + "_invalid";

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "ConfirmPassword", "Passwords do not match");
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResultWithErrorIfNewPasswordIsNotBetween8And16Characters()
        {
            // Arrange
            _command.NewPassword = "1234567";

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "NewPassword", "Password does not meet requirements");
        }

        [TestCase("abcdefghijk1")]
        [TestCase("ABCDEFGHIJK1")]
        public async Task ThenItShouldReturnAnInvalidResultWithErrorIfNewPasswordDoesNotHave1UpperAnd1Lower(string password)
        {
            // Arrange
            _command.NewPassword = password;

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "NewPassword", "Password does not meet requirements");
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResultWithErrorIfNewPasswordDoesNotContainAtLeast1Number()
        {
            // Arrange
            _command.NewPassword = "abcdefghijk";

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "NewPassword", "Password does not meet requirements");
        }

        [Test]
        public async Task ThenItShouldReturnAnInvalidResultWithErrorIfNewPasswordIsInConfiguredRecentHistory()
        {
            // Arrange
            _command.User.PasswordHistory = new[]
            {
                new Domain.HistoricalPassword { Password = "HistoricalPassword1", Salt = "Salt1", PasswordProfileId = PasswordProfileId, DateSet = new DateTime(2016, 1, 1) }
            };
            _passwordService.Setup(s => s.VerifyAsync(NewPassword, "HistoricalPassword1", "Salt1", PasswordProfileId))
                .Returns(Task.FromResult(true));

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            AssertExpectedValidationErrorExists(actual, "NewPassword", "Password has been used too recently. You cannot use your last 3 passwords");
        }

        [Test]
        public async Task ThenItShouldReturnAnValidResultWithErrorIfNewPasswordIsInHistoryButNotConfiguredRecentHistory()
        {
            // Arrange
            _command.User.PasswordHistory = new[]
            {
                new Domain.HistoricalPassword { Password = "HistoricalPassword1", Salt = "Salt1", PasswordProfileId = PasswordProfileId, DateSet = new DateTime(2016, 1, 1) },
                new Domain.HistoricalPassword { Password = "HistoricalPassword2", Salt = "Salt2", PasswordProfileId = PasswordProfileId, DateSet = new DateTime(2016, 2, 1) },
                new Domain.HistoricalPassword { Password = "HistoricalPassword3", Salt = "Salt3", PasswordProfileId = PasswordProfileId, DateSet = new DateTime(2016, 3, 1) },
                new Domain.HistoricalPassword { Password = "HistoricalPassword4", Salt = "Salt4", PasswordProfileId = PasswordProfileId, DateSet = new DateTime(2016, 4, 1) },
            };
            _passwordService.Setup(s => s.VerifyAsync(NewPassword, "HistoricalPassword1", "Salt1", PasswordProfileId))
                .Returns(Task.FromResult(true));

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [TestCase("password", "", false, true)]
        [TestCase("password", null, false, true)]
        [TestCase("", "password", true, false)]
        [TestCase(null, "password", true, false)]
        [TestCase("", "", true, true)]
        [TestCase(null, null, true, true)]
        public async Task ThenItShouldReturnAnInvalidModelWhenValuesMissing(string currentPassword, string newPassword, bool expectCurrentPasswordError, bool expectNewPasswordError)
        {
            // Arrange
            _command.CurrentPassword = currentPassword;
            _command.NewPassword = newPassword;

            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            if (expectCurrentPasswordError)
            {
                Assert.IsTrue(actual.ValidationDictionary.ContainsKey("CurrentPassword"));
            }
            if (expectNewPasswordError)
            {
                Assert.IsTrue(actual.ValidationDictionary.ContainsKey("NewPassword"));
            }
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
