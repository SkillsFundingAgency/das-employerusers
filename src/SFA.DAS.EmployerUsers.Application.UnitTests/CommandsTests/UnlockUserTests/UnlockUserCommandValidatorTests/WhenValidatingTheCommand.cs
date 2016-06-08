using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.UnlockUser;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.UnlockUserTests.UnlockUserCommandValidatorTests
{
    public class WhenValidatingTheCommand
    {
        private UnlockUserCommandValidator _unlockUserCommandValidator;

        [SetUp]
        public void Arrange()
        {
            _unlockUserCommandValidator = new UnlockUserCommandValidator();
        }

        [Test]
        public void ThenTheDictionaryIsPopulatedIfTheCommandIsNotPopulated()
        {
            //Act
            var actual = _unlockUserCommandValidator.Validate(new UnlockUserCommand());

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTheDicionaryIsEmptyIsTheCommmandIsPopulated()
        {
            //Act
            var actual =
                _unlockUserCommandValidator.Validate(new UnlockUserCommand
                {
                    Email = "test@local",
                    UnlockCode = "SomeCode"
                });

            //Assert
            Assert.IsEmpty(actual.ValidationDictionary);
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenTheDictionaryContainsTheCorrectErrorMessagesWhenNotValid()
        {
            //Act
            var actual = _unlockUserCommandValidator.Validate(new UnlockUserCommand());

            //Assert
            Assert.IsNotEmpty(actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("Email", "Email has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("UnlockCode", "Unlock Code has not been supplied"), actual.ValidationDictionary);
        }
    }
}
