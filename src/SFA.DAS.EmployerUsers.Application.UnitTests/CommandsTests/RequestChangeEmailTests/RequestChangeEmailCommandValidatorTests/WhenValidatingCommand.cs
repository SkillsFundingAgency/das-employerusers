using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.RequestChangeEmail;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RequestChangeEmailTests.RequestChangeEmailCommandValidatorTests
{
    public class WhenValidatingCommand
    {
        private RequestChangeEmailCommand _command;
        private RequestChangeEmailCommandValidator _validator;
        private Mock<IUserRepository> _userRepository;

        [SetUp]
        public void Arrange()
        {
            _command = new RequestChangeEmailCommand
            {
                UserId = "USER1",
                NewEmailAddress = "user.one@unit.tests",
                ConfirmEmailAddress = "user.one@unit.tests"
            };

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>())).ReturnsAsync((User)null);

            _validator = new RequestChangeEmailCommandValidator(_userRepository.Object);
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
            Assert.AreEqual("Emails don't match", actual.ValidationDictionary["ConfirmEmailAddress"]);
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
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey("UserId"));
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
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey("NewEmailAddress"));
        }

        [Test]
        public async Task ThenIfTheEmailIsAlreadyRegisteredAnErrorIsReturned()
        {
            //Arrange
            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>())).ReturnsAsync(new User());

            // Act
            var actual = await _validator.ValidateAsync(_command);

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey("NewEmailAddress"));
        }
        
    }
}
