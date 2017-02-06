using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RegisterUserTests.RegisterUserCommandValidatorTests
{
    public class WhenValidatingTheRequest
    {
        private RegisterUserCommandValidator _validator;
        private Mock<IPasswordService> _passwordService;

        [SetUp]
        public void Arrange()
        {
            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(x => x.CheckPasswordMatchesRequiredComplexity(It.IsAny<string>())).Returns(true);

            _validator = new RegisterUserCommandValidator(_passwordService.Object);
        }

        [Test]
        public async Task ThenAValidationResultObjectThatIsValidIsReturnedIfAllFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new RegisterUserCommand
            {
                Email = "test@example.com",
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



        [Test]
        public async Task ThenAValidationResultObjectThatIsNotValidIsReturnedIfEmailIsInvalid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new RegisterUserCommand
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
            Assert.IsFalse(actual.IsValid());
        }



        [TestCase("", "", "", "", "")]
        [TestCase(" ", " ", " ", " ", " ")]
        [TestCase("aaa", "", "", "", "")]
        [TestCase("", "aaa", "", "", "")]
        [TestCase("", "", "aaa", "", "")]
        [TestCase("", "", "aaa", "aaa", "")]
        [TestCase("", "aaa", "aaa", "aaa", "")]
        [TestCase("aaa", "aaa", "aaa", "aaa", "")]
        public async Task ThenFalseIsReturnedIfThereAreMissingFields(string firstName, string lastName, string email, string password, string confirmPassword)
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
            var actual = await _validator.ValidateAsync(registerUserCommand);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }
        
        [Test]
        public async Task ThenFalseIsReturnedIftheConfirmPasswordIsNull()
        {
            //Act
            var actual = await _validator.ValidateAsync(new RegisterUserCommand
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
        public async Task ThenFalseIsReturnedIftheTermsAndConditionsHaveNotBeenAccepted()
        {
            //Act
            var actual = await _validator.ValidateAsync(new RegisterUserCommand
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
        public async Task ThenTheDictionaryIsPopulatedWithTheFailedFieldAndReasonWhenTheCommandIsNotValid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new RegisterUserCommand
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
            Assert.Contains(new KeyValuePair<string, string>("Email", "Enter a valid email address"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("FirstName", "Enter first name"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("LastName", "Enter last name"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("Password", "Enter password"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("ConfirmPassword", "Re-type password"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HasAcceptedTermsAndConditions", "You need to accept the terms of use"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenThedictionaryIsPopulatedWithTheFailedMessagesWhenThePasswordValidationHasFailed()
        {
            //Arrange
            _passwordService.Setup(x => x.CheckPasswordMatchesRequiredComplexity(It.IsAny<string>())).Returns(false);

            //Act
            var actual = await _validator.ValidateAsync(new RegisterUserCommand
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
        public async Task ThenThedictionaryIsPopulatedWithTheFailedMessagesWhenThePasswordsDoNotMatch()
        {
            //Act
            var actual = await _validator.ValidateAsync(new RegisterUserCommand
            {
                Email = "a",
                FirstName = "a",
                LastName = "a",
                Password = "Pa55word",
                ConfirmPassword = "b"
            });

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("ConfirmPassword", "Passwords don't match"), actual.ValidationDictionary);
        }

    }
}
