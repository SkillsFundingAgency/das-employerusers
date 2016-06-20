using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.PasswordResetTests.PasswordResetCommandValidatorTests
{
    public class WhenValidatingTheRequest
    {
        private PasswordResetCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new PasswordResetCommandValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheMessageHasNoUser()
        {
            //Act
            var actual = _validator.Validate(new PasswordResetCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedIfThePasscodeDoesNotMatch()
        {
            //Act
            var actual = _validator.Validate(new PasswordResetCommand { PasswordResetCode = "123456", User = new User { PasswordResetCode = "654321" } });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PasswordResetCode", "Reset code is invalid, try again"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenFalseIsReturnedfIfThePasswordsDoNotMatch()
        {
            //Act
            var actual = _validator.Validate(new PasswordResetCommand {
                PasswordResetCode = "654321",
                Password = "654321abc",
                ConfirmPassword = "654321aBc",
                User = new User { PasswordResetCode = "654321" } });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("ConfirmPassword", "Sorry, your passwords don’t match"), actual.ValidationDictionary );
        }

        [Test] public void ThenTrueIsReturnedIfAllFieldsHaveBeenSupplied()
        {
            //Act
            var actual = _validator.Validate(new PasswordResetCommand
            {
                PasswordResetCode = "123456ABC",
                Password = "abc123YHN",
                ConfirmPassword = "abc123YHN",
                User = new User
                {
                    PasswordResetCode = "123456abc"
                }
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
        
    }
}
