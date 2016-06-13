using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void ThenFalseIsReturnedIfTheValuesAreEmpty()
        {
            //Act
            var actual = _validator.Validate(new ResendUnlockCodeCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]

        public void ThenTrueIsReturnedIfTheValuesAreNotEmpty()
        {
            //act
            var actual = _validator.Validate(new ResendUnlockCodeCommand {Email = "someEmail"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenTheErrorDictionaryIsPopulatedCorrectlyWhenThereIsAnError()
        {
            //Act
            var actual = _validator.Validate(new ResendUnlockCodeCommand());

            //Assert
            Assert.IsNotNull(actual);
            Assert.Contains(new KeyValuePair<string,string>("Email", "Please enter email address"), actual.ValidationDictionary);
        }
    }
}
