using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Application.Validation;

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
        public void ThenAValidationResultObjectThatIsValidIsReturnedIfAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "test",
                FirstName = "Testing",
                LastName = "Tester",
                Password = "p24234AAA",
                ConfirmPassword = "p24234AAA",
                HasAcceptedTermsAndConditions = true
            });

            //Assert
            Assert.IsAssignableFrom<ValidationResult>(actual);
            Assert.IsTrue(actual.IsValid());
        }

        [TestCase("", "", "", "", "")]
        [TestCase(" ", " ", " ", " ", " ")]
        [TestCase("aaa", "", "", "", "")]
        [TestCase("", "aaa", "", "", "")]
        [TestCase("", "", "aaa", "", "")]
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
                ConfirmPassword = confirmPassword,
                HasAcceptedTermsAndConditions = true
            };

            //Act
            var actual = _validator.Validate(registerUserCommand);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }
        
        [TestCase("Passw0r")]
        [TestCase("Password")]
        [TestCase("123456789")]
        [TestCase("aaaaa6789")]
        [TestCase("AAAAA6789")]
        [TestCase("")]
        [TestCase(null)]
        public void ThenFalseIsReturnedIfThePasswordDoesNotTheRequiredStrenth(string password)
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "test",
                FirstName = "Testing",
                LastName = "Tester",
                Password = password,
                ConfirmPassword = password,
                HasAcceptedTermsAndConditions = true
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedIftheConfirmPasswordIsNull()
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "test",
                FirstName = "Testing",
                LastName = "Tester",
                Password = "P@55w0rd",
                ConfirmPassword = null,
                HasAcceptedTermsAndConditions = true
            });

            //Assert
            Assert.IsFalse(actual.IsValid());

        }

        [Test]
        public void ThenFalseIsReturnedIftheTermsAndConditionsHaveNotBeenAccepted()
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "test",
                FirstName = "Testing",
                LastName = "Tester",
                Password = "P@55w0rd",
                ConfirmPassword = "P@55w0rd",
                HasAcceptedTermsAndConditions = false
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTheDictionaryIsPopulatedWithTheFailedFieldAndReasonWhenTheCommandIsNotValid()
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "",
                FirstName = "",
                LastName = "",
                Password = "",
                ConfirmPassword = "",
                HasAcceptedTermsAndConditions = false
            });

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("Email", "Please enter email address"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("FirstName", "Please enter first name"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("LastName", "Please enter last name"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("Password", "Please enter password"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("ConfirmPassword", "Please confirm password"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HasAcceptedTermsAndConditions", "Please accept the terms and conditions"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenThedictionaryIsPopulatedWithTheFailedMessagesWhenThePasswordValidationHasFailed()
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "a",
                FirstName = "a",
                LastName = "a",
                Password = "a",
                ConfirmPassword = "b"
            });

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("Password", "Password requires upper and lowercase letters, a number and at least 8 characters"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenThedictionaryIsPopulatedWithTheFailedMessagesWhenThePasswordsDoNotMatch()
        {
            //Act
            var actual = _validator.Validate(new RegisterUserCommand
            {
                Email = "a",
                FirstName = "a",
                LastName = "a",
                Password = "Pa55word",
                ConfirmPassword = "b"
            });

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("ConfirmPassword", "Sorry, your passwords don’t match"), actual.ValidationDictionary);
        }

    }
}
