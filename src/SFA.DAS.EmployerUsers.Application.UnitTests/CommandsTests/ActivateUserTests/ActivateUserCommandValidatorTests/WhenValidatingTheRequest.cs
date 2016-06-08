using System;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ActivateUserTests.ActivateUserCommandValidatorTests
{
    public class WhenValidatingTheRequest
    {
        private ActivateUserCommandValidator _activateUserCommandValidator;

        [SetUp]
        public void Arrange()
        {
            _activateUserCommandValidator = new ActivateUserCommandValidator();
        }

        [Test]
        public void ThenFalseIsReturnedIfAllFieldsArentPopulated()
        {
            //Act
            var actual = _activateUserCommandValidator.Validate(new ActivateUserCommand());

            //Assert
            Assert.IsAssignableFrom<ValidationResult>(actual);
            Assert.IsFalse(actual.IsValid());
        }


        [Test]
        public void ThenFalseIsReturnedIfNullIsPassed()
        {
            //Act
            var actual = _activateUserCommandValidator.Validate(null);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTrueIsReturnedIfAllFieldsAreProvidedAndTheAccessCodeMatchesCaseInsensitive()
        {
            //Act
            var actual = _activateUserCommandValidator.Validate(new ActivateUserCommand {AccessCode = "AccessCode", UserId = Guid.NewGuid().ToString(), User = new User {AccessCode = "ACCESSCODE"} });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedIfTheAccessCodeDoesntMatchTheOneOnTheUser()
        {
            //Act
            var actual = _activateUserCommandValidator.Validate(new ActivateUserCommand { AccessCode = "AccessCode", UserId = Guid.NewGuid().ToString(), User = new User {AccessCode = "Edocssecca"} });

            //Assert
            Assert.IsFalse(actual.IsValid());
        }
    }
}
