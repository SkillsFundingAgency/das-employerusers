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
            var command = new ActivateUserCommand
            {
                AccessCode = "AccessCode",
                UserId = Guid.NewGuid().ToString(),
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "ACCESSCODE",
                            CodeType = SecurityCodeType.AccessCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            };
            var actual = _activateUserCommandValidator.Validate(command);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedIfTheAccessCodeDoesntMatchTheAnyOnTheUser()
        {
            //Act
            var command = new ActivateUserCommand
            {
                AccessCode = "AccessCode",
                UserId = Guid.NewGuid().ToString(),
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "Edocssecca",
                            CodeType = SecurityCodeType.AccessCode,
                            ExpiryTime = DateTime.MaxValue
                        }
                    }
                }
            };
            var actual = _activateUserCommandValidator.Validate(command);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedIfTheAccessCodeMatchAnyOnTheUserButItHasExpired()
        {
            //Act
            var command = new ActivateUserCommand
            {
                AccessCode = "AccessCode",
                UserId = Guid.NewGuid().ToString(),
                User = new User
                {
                    SecurityCodes = new[]
                    {
                        new SecurityCode
                        {
                            Code = "AccessCode",
                            CodeType = SecurityCodeType.AccessCode,
                            ExpiryTime = DateTime.MinValue
                        }
                    }
                }
            };
            var actual = _activateUserCommandValidator.Validate(command);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTrueIsReturnedIfOnlyTheEmailHasBeenSuppliedAndTheUserIdIsNull()
        {
            //Act
            var command = new ActivateUserCommand
            {
                UserId = null,
                Email = "User@Email",
                User = new User
                {
                    Email = "user@email"
                }
            };
            var actual = _activateUserCommandValidator.Validate(command);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
