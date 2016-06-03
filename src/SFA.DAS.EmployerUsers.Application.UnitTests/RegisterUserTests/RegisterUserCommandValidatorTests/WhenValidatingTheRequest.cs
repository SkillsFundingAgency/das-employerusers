using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.RegisterUserTests.RegisterUserCommandValidatorTests
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
        public void ThenAnEmptyDictionaryIsReturnedIfAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "test",
                FirstName = "Testing",
                LastName = "Tester",
                Password = "p24234AAA",
                ConfirmPassword = "p24234AAA"
            });

            //Assert
            Assert.IsAssignableFrom<Dictionary<string,string>>(actual);
            Assert.IsEmpty(actual);
        }

        [TestCase("", "", "", "", "")]
        [TestCase(" ", " ", " ", " ",  " ")]
        [TestCase("aaa", "", "", "",  "")]
        [TestCase("", "aaa", "", "",  "")]
        [TestCase("", "", "aaa", "",  "")]
        [TestCase("", "", "aaa", "aaa", "")]
        [TestCase("", "aaa", "aaa", "aaa", "")]
        [TestCase("aaa", "aaa", "aaa", "aaa", "")]
        public void ThenFalseIsReturnedIfThereAreMissingFields(string firstName, string lastName, string email, string password, string confirmPassword)
        {
            //Arrange
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            //Act
            var actual = _validator.Validate(registerUserCommand);

            //Assert
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public void ThenFalseIsReturnedIfNullIsPassed()
        {
            //Act
            var actual = _validator.Validate(null);

            //Assert
            Assert.IsNotEmpty(actual);

        }

        [TestCase("Passw0r")]
        [TestCase("Password")]
        [TestCase("123456789")]
        [TestCase("aaaaa6789")]
        [TestCase("AAAAA6789")]
        public void ThenFalseIsReturnedIfThePasswordDoesNotTheRequiredStrenth(string password)
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "test",
                FirstName = "Testing",
                LastName = "Tester",
                Password = password,
                ConfirmPassword = password
            });

            //Assert
            Assert.IsNotEmpty(actual);
        }
        
    }
}
