using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.ResendActivationCode;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.ResendActivationCodeTests.ResendActivationCodeCommandValidatorTests
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
        public async Task ThenTrueIsReturnedIfValidCommandIsPassed()
        {
            var result = await _validator.ValidateAsync(new ResendActivationCodeCommand
            {
                UserId = Guid.NewGuid().ToString()
            });
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfEmptyCommandIsPassed()
        {
            var result = await _validator.ValidateAsync(new ResendActivationCodeCommand());
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfNullIsPassed()
        {
            var result = await _validator.ValidateAsync(null);
            Assert.IsFalse(result.IsValid());
        }
    }
}