using NUnit.Framework;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.UnitTests.RegisterUserTests.RegisterUserCommandValidatorTests
{
    public class WhenValidatingTheRequest
    {
        private RegisterUserCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new RegisterUserCommandValidator();
        }

        [Test]
        public void ThenTrueIsReturnedIfAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "test",
                FirstName = "Testing",
                LastName = "Tester",
                Password = "p$24234AAA",
                ConfirmPassword = "p$24234AAA",
                ConfirmEmail = "test"
            });

            //Assert
            Assert.IsTrue(actual);
        }

        [TestCase("", "", "", "", "", "")]
        [TestCase(" ", " ", " ", " ", " ", " ")]
        [TestCase("aaa", "", "", "", "", "")]
        [TestCase("", "aaa", "", "", "", "")]
        [TestCase("", "", "aaa", "", "", "")]
        [TestCase("", "", "aaa", "aaa", "", "")]
        [TestCase("", "aaa", "aaa", "aaa", "", "")]
        [TestCase("aaa", "aaa", "aaa", "aaa", "", "")]
        [TestCase("aaa", "aaa", "aaa", "aaa", "aaa", "")]
        [TestCase("aaa", "aaa", "aaa", "aaa", "", "aaa")]
        [TestCase("aaa", "aaa", "aaa", "aaa", "bbb", "bbb")]
        [TestCase("aaa", "aaa", "aaa", "aaa", "aaa", "AAA")]
        public void ThenFalseIsReturnedIfThereAreMissingFields(string firstName, string lastName, string email, string password, string confirmEmail, string confirmPassword)
        {
            //Arrange
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                ConfirmEmail = confirmEmail,
                ConfirmPassword = confirmPassword
            };

            //Act
            var actual = _validator.Validate(registerUserCommand);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void ThenFalseIsReturnedIfNullIsPassed()
        {
            //Act
            var actual = _validator.Validate(null);

            //Assert
            Assert.IsFalse(actual);

        }
    }
}
