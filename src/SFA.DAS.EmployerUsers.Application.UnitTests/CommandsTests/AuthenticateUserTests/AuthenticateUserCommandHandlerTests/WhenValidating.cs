using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.AuthenticateUser;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.AuthenticateUserTests.AuthenticateUserCommandHandlerTests
{
    public class WhenValidating
    {
        private AuthenticateUserCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new AuthenticateUserCommandValidator();
        }

        [Test]
        public async Task ThenTheCommandIsNotValidWhenTheFieldsAreNotPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new AuthenticateUserCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("EmailAddress", "Enter a valid email address"),actual.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("Password", "Enter password"),actual.ValidationDictionary );
        }

        [Test]
        public async Task ThenTheCommandIsNotValidWhenTheEmailIsNotInTheCorrectFormat()
        {
            //Act
            var actual = await _validator.ValidateAsync(new AuthenticateUserCommand {EmailAddress = "test",Password = "testPassword"});

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("EmailAddress", "Enter a valid email address"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheCommandIsValidWhenAllFieldsArePopulatedAndTheEmailIsValid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new AuthenticateUserCommand { EmailAddress = "tes't@test.com", Password = "testPassword" });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
