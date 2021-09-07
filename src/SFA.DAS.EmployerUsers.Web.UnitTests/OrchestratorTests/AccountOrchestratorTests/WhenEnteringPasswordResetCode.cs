using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
using SFA.DAS.EmployerUsers.Application.Exceptions;
using SFA.DAS.EmployerUsers.Application.Queries.GetUnlockCodeLength;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenEnteringPasswordResetCode
    {
        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<ILogger> _logger;
        private const string ValidEmail = "somevalidemail@local";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserByEmailAddressQuery>())).ReturnsAsync(new User());
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUnlockCodeQuery>())).ReturnsAsync(new GetUnlockCodeResponse{UnlockCodeLength = 99});
            _logger = new Mock<ILogger>();

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object,_logger.Object);
        }

        [Test]
        public async Task ThenTheCommandIsCalledByTheMediator()
        {
            // Arrange
            var actualResetCode = "123456";
            var model = new EnterResetCodeViewModel {  Email = ValidEmail, PasswordResetCode = actualResetCode };

            // Act
            await _accountOrchestrator.ValidateResetCode(model);

            // Assert
            _mediator.Verify(x => x.SendAsync(It.Is<ValidatePasswordResetCodeCommand>(c => c.Email == ValidEmail && c.PasswordResetCode == actualResetCode)), Times.Once);
        }

        [Test]
        public async Task ThenTheErrorDictionaryIsPopulatedIfAnInvalidRequestExceptionIsThrown()
        {
            // Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePasswordResetCodeCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "ConfrimPassword", "Some Error" } }));

            // Act
            var actual = await _accountOrchestrator.ValidateResetCode(new EnterResetCodeViewModel());

            // Assert
            Assert.IsNotEmpty(actual.FlashMessage.ErrorMessages);
        }

        [Test]
        public async Task ThenTheErrorDictionaryContainsTheFieldErrors()
        {
            // Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePasswordResetCodeCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>
            {
                { "PasswordResetCode", "Some Reset Code Error" }
            }));


            // Act
            var actual = await _accountOrchestrator.ValidateResetCode(new EnterResetCodeViewModel());

            // Assert
            Assert.AreEqual("Some Reset Code Error", actual.Data.PasswordResetCodeError);
        }
    }
}
