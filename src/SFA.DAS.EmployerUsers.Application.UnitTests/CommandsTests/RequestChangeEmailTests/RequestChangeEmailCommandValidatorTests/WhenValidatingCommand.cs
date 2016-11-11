using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.RequestChangeEmail;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RequestChangeEmailTests.RequestChangeEmailCommandValidatorTests
{
    public class WhenValidatingCommand
    {
        private RequestChangeEmailCommand _command;
        private RequestChangeEmailCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _command = new RequestChangeEmailCommand
            {
                UserId = "USER1",
                NewEmailAddress = "user.one@unit.tests",
                ConfirmEmailAddress = "user.one@unit.tests"
            };

            _validator = new RequestChangeEmailCommandValidator();
        }

        [Test]
        public void ThenItShouldReturnAnInstanceOfValidationResult()
        {
            // Act
            var actual = _validator.Validate(_command);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public void ThenItShouldReturnAValidResponseIfNoErrors()
        {
            // Act
            var actual = _validator.Validate(_command);

            // Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenItShouldReturnInvalidAndAnErrorWhenEmailsDoNotMatch()
        {
            // Arrange
            _command.NewEmailAddress = "user.one@unit.tests";
            _command.ConfirmEmailAddress = "user.two@unit.tests";
            // Act
            var actual = _validator.Validate(_command);

            // Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey("ConfirmEmailAddress"));
            Assert.AreEqual("Confirm email address does not match new email address", actual.ValidationDictionary["ConfirmEmailAddress"]);
        }

        [Test]
        public void ThenItShouldReturnInvalidAndAnErrorWhenUserIdIsNotSpecified()
        {
            // Arrange
            _command.UserId = null;
            // Act
            var actual = _validator.Validate(_command);

            // Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey(""));
        }

        [Test]
        public void ThenItShouldReturnInvalidAndAnErrorWhenNewEmailAddressIsNotSpecified()
        {
            // Arrange
            _command.NewEmailAddress = string.Empty;
            // Act
            var actual = _validator.Validate(_command);

            // Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey(""));
        }

    }
}
