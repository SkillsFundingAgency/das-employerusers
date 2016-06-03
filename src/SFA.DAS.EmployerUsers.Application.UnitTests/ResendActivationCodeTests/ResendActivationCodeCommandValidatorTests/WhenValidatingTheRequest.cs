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
        public void ThenTrueIsReturnedIfValidCommandIsPassed()
        {
            Assert.IsTrue(_validator.Validate(new ResendActivationCodeCommand
            {
                UserId = Guid.NewGuid().ToString()
            }).IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedIfEmptyCommandIsPassed()
        {
            Assert.IsFalse(_validator.Validate(new ResendActivationCodeCommand()).IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedIfNullIsPassed()
        {
            Assert.IsFalse(_validator.Validate(null).IsValid());
        }
    }
}