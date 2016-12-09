using System.Threading.Tasks;
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
        public async Task ThenItShouldReturnAnInstanceOfValidationResult()
        {
            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnAValidResponseIfNoErrors()
        {
            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenItShouldReturnInvalidAndAnErrorWhenEmailsDoNotMatch()
        {
            // Arrange
            _command.NewEmailAddress = "user.one@unit.tests";
            _command.ConfirmEmailAddress = "user.two@unit.tests";
            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey("ConfirmEmailAddress"));
            Assert.AreEqual("Confirm email address does not match new email address", actual.ValidationDictionary["ConfirmEmailAddress"]);
        }

        [Test]
        public async Task ThenItShouldReturnInvalidAndAnErrorWhenUserIdIsNotSpecified()
        {
            // Arrange
            _command.UserId = null;
            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey(""));
        }

        [Test]
        public async Task ThenItShouldReturnInvalidAndAnErrorWhenNewEmailAddressIsNotSpecified()
        {
            // Arrange
            _command.NewEmailAddress = string.Empty;
            // Act
            var actual = await _validator.ValidateAsync(_command);

            // Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey(""));
        }

    }
}
