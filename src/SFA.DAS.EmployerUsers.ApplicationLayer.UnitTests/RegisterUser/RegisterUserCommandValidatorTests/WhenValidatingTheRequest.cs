using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.UnitTests.RegisterUser.RegisterUserCommandValidatorTests
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
                Password = "p$24234AAA"
            });

            //Assert
            Assert.IsTrue(actual);
        }

        [TestCase("", "", "", "")]
        [TestCase(" ", " ", " ", " ")]
        [TestCase("aaa", "", "", "")]
        [TestCase("", "aaa", "", "")]
        [TestCase("", "", "aaa", "")]
        [TestCase("", "", "aaa", "aaa")]
        [TestCase("", "aaa", "aaa", "aaa")]
        public void ThenFalseIsReturnedIfThereAreMissingFields(string firstName, string lastName, string email, string password)
        {
            //Arrange
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password
            };

            //Act
            var actual = _validator.Validate(registerUserCommand);

            //Assert
            Assert.IsFalse(actual);
        }
    }
}
