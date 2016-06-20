using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
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

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, null);
        }

        [Test]
        public async Task ThenTheCommandIsCalledByTheMediator()
        {
            //Arrange
            var actualResetCode = "123456";
            var model = new PasswordResetViewModel { Email = ValidEmail, PasswordResetCode = actualResetCode, Password = "password", ConfirmPassword = "passwordconfirm" };

            //Act
            await _accountOrchestrator.PasswordResetCodeCommand(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<PasswordResetCommand>(c => c.Email == ValidEmail && c.Password=="password" && c.ConfirmPassword=="passwordconfirm" && c.PasswordResetCode == actualResetCode)), Times.Once);

        }
        
        [Test]
        public async Task ThenTheErrorDictionaryIsPopulatedIfAnExceptionIsThrown()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<PasswordResetCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "ConfrimPassword", "Some Error" } }));

            //Act
            var actual = await _accountOrchestrator.PasswordResetCodeCommand(new PasswordResetViewModel());

            //Assert
            Assert.IsNotEmpty(actual.ErrorDictionary);
            Assert.IsFalse(actual.Valid);
        }

        [Test]
        public async Task ThenTheErrorDictionaryContainsTheFieldErrors()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<PasswordResetCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>
            {
                { "ConfirmPassword", "Some Confirm Error" },
                { "PasswordResetCode", "Some Password Reset Error" }
            }));


            //Act
            var actual = await _accountOrchestrator.PasswordResetCodeCommand(new PasswordResetViewModel());

            //Assert
            Assert.AreEqual("Some Confirm Error", actual.ConfirmPasswordError);
            Assert.AreEqual("Some Password Reset Error", actual.PasswordResetCodeError);
        }
    }
}
