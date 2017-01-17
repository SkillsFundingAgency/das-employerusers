using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ResendUnlockCode;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ResendUnlockCodeTests.ResendUnlockCodeCommandValidatorTests
{
    public class WhenValidatingTheCommand
    {
        private ResendUnlockCodeCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new ResendUnlockCodeCommandValidator();
        }

        [Test]
        public async Task ThenFalseIsReturnedIfTheValuesAreEmpty()
        {
            //Act
            var actual = await _validator.ValidateAsync(new ResendUnlockCodeCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]

        public async Task ThenTrueIsReturnedIfTheValuesAreNotEmpty()
        {
            //act
            var actual = await _validator.ValidateAsync(new ResendUnlockCodeCommand {Email = "someEmail@local.com"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfTheEmailAddressIsNotValid()
        {
            //act
            var actual = await _validator.ValidateAsync(new ResendUnlockCodeCommand { Email = "someEmail" });

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenTheErrorDictionaryIsPopulatedCorrectlyWhenThereIsAnError()
        {
            //Act
            var actual = await _validator.ValidateAsync(new ResendUnlockCodeCommand());

            //Assert
            Assert.IsNotNull(actual);
            Assert.Contains(new KeyValuePair<string,string>("Email", "Please enter email address"), actual.ValidationDictionary);
        }
    }
}
