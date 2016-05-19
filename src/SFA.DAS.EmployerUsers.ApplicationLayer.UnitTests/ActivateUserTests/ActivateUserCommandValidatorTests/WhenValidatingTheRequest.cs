using System;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.ActivateUser;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.UnitTests.ActivateUserTests.ActivateUserCommandValidatorTests
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
            Assert.IsFalse(actual);
        }


        [Test]
        public void ThenFalseIsReturnedIfNullIsPassed()
        {
            //Act
            var actual = _activateUserCommandValidator.Validate(null);

            //Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void ThenTrueIsReturnedIfAllFieldsAreProvidedAndTheAccessCodeMatchesCaseInsensitive()
        {
            //Act
            var actual = _activateUserCommandValidator.Validate(new ActivateUserCommand {AccessCode = "AccessCode", UserId = Guid.NewGuid().ToString(), User = new User {AccessCode = "ACCESSCODE"} });

            //Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void ThenFalseIsReturnedIfTheAccessCodeDoesntMatchTheOneOnTheUser()
        {
            //Act
            var actual = _activateUserCommandValidator.Validate(new ActivateUserCommand { AccessCode = "AccessCode", UserId = Guid.NewGuid().ToString(), User = new User {AccessCode = "Edocssecca"} });

            //Assert
            Assert.IsFalse(actual);
        }
    }
}
