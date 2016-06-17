using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.EmployerUsers.Application.Queries.IsPasswordResetValid;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenEnteringPasswordResetCode
    {
        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private const string ValidEmail = "somevalidemail@local";
        private const string InValidEmail = "someinvalidemail@local";
        private const string ExpiredValidEmail = "someexpiredemail@local";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.Is<IsPasswordResetCodeValidQuery>(c => c.Email == ValidEmail))).ReturnsAsync(new PasswordResetCodeResponse { IsValid = true, HasExpired = false });
            _mediator.Setup(x => x.SendAsync(It.Is<IsPasswordResetCodeValidQuery>(c => c.Email == InValidEmail))).ReturnsAsync(new PasswordResetCodeResponse { IsValid = false, HasExpired = false });
            _mediator.Setup(x => x.SendAsync(It.Is<IsPasswordResetCodeValidQuery>(c => c.Email == ExpiredValidEmail))).ReturnsAsync(new PasswordResetCodeResponse { IsValid = false, HasExpired = true });

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, null);
        }

        [Test]
        public async Task ThenTheQueryIsCalledByTheMediator()
        {
            //Arrange
            var actualResetCode = "123456";
            var model = new PasswordResetViewModel { Email = ValidEmail, PasswordResetCode = actualResetCode };

            //Act
            await _accountOrchestrator.PasswordReset(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<IsPasswordResetCodeValidQuery>(c => c.Email == ValidEmail && c.PasswordResetCode == actualResetCode)), Times.Once);

        }

        [TestCase(ValidEmail, true, false)]
        [TestCase(InValidEmail, false, false)]
        [TestCase(ExpiredValidEmail, false, true)]
        public async Task ThenTheReturnModelsPopulatedCorrectlyFromTheQuery(string email, bool isValid, bool hasExpired)
        {
            //Arrange
            var model = new PasswordResetViewModel { Email = email};

            //Act
            var actual = await _accountOrchestrator.PasswordReset(model);

            //Assert
            Assert.AreEqual(isValid, actual.IsValid);
            Assert.AreEqual(hasExpired, actual.HasExpired);
            
        }
    }
}
