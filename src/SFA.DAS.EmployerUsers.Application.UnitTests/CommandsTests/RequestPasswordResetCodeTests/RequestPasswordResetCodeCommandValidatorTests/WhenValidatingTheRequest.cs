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
        public void ThenTrueIsReturnedIfValidCommandIsPassed()
        {
            var validationResult = _validator.Validate(new RequestPasswordResetCodeCommand
            {
                Email = "test.user@test.org"
            });

            Assert.IsTrue(validationResult.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedIfEmptyCommandIsPassed()
        {
            var validationResult = _validator.Validate(new RequestPasswordResetCodeCommand());

            Assert.IsFalse(validationResult.IsValid());
        }
    }
}