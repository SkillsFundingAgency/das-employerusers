using System;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.ResendActivationCodeTests.ResendActivationCodeCommandValidatorTests
{
    [TestFixture]
    public class WhenValidatingTheRequest
    {
        private ResendActivationCodeCommandValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new ResendActivationCodeCommandValidator();
        }

        [Test]
        public void ThenFalseIsReturnedIfValidCommandIsPassed()
        {
            Assert.True(_validator.Validate(new ResendActivationCodeCommand
            {
                UserId = Guid.NewGuid().ToString()
            }));
        }

        [Test]
        public void ThenFalseIsReturnedIfEmptyCommandIsPassed()
        {
            Assert.False(_validator.Validate(new ResendActivationCodeCommand()));
        }

        [Test]
        public void ThenFalseIsReturnedIfNullIsPassed()
        {
            Assert.False(_validator.Validate(null));
        }
    }
}