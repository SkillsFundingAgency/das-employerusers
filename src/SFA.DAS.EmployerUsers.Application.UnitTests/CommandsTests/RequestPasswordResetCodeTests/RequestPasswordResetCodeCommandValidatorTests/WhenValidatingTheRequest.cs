using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.RequestPasswordResetCode;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RequestPasswordResetCodeTests.RequestPasswordResetCodeCommandValidatorTests
{
    [TestFixture]
    public class WhenValidatingTheRequest
    {
        private RequestPasswordResetCodeCommandValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new RequestPasswordResetCodeCommandValidator();
        }

        [Test]
        public async Task ThenTrueIsReturnedIfValidCommandIsPassed()
        {
            var validationResult = await _validator.ValidateAsync(new RequestPasswordResetCodeCommand
            {
                Email = "test.user@test.org"
            });

            Assert.IsTrue(validationResult.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedIfEmptyCommandIsPassed()
        {
            var validationResult = await _validator.ValidateAsync(new RequestPasswordResetCodeCommand());

            Assert.IsFalse(validationResult.IsValid());
        }
    }
}