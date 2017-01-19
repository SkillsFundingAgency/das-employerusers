using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
using SFA.DAS.EmployerUsers.Application.Queries.GetUserByEmailAddress;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

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
            _logger = new Mock<ILogger>();

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object,_logger.Object);
        }

        [Test]
        public async Task ThenTheCommandIsCalledByTheMediator()
        {
            //Arrange
            
            var actualResetCode = "123456";
            var model = new PasswordResetViewModel {  Email = ValidEmail, PasswordResetCode = actualResetCode, Password = "password", ConfirmPassword = "passwordconfirm" };

            //Act
            await _accountOrchestrator.ResetPassword(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<PasswordResetCommand>(c => c.Email == ValidEmail && c.Password=="password" && c.ConfirmPassword=="passwordconfirm" && c.PasswordResetCode == actualResetCode)), Times.Once);

        }
        
        [Test]
        public async Task ThenTheErrorDictionaryIsPopulatedIfAnExceptionIsThrown()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<PasswordResetCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "ConfrimPassword", "Some Error" } }));

            //Act
            var actual = await _accountOrchestrator.ResetPassword(new PasswordResetViewModel());

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
            var actual = await _accountOrchestrator.ResetPassword(new PasswordResetViewModel());

            //Assert
            Assert.AreEqual("Some Confirm Error", actual.ConfirmPasswordError);
            Assert.AreEqual("Some Password Reset Error", actual.PasswordResetCodeError);
        }

        [Test]
        public async Task ThenThePasswordFieldsAreEmptiedIfThereAreErrors()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<PasswordResetCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>
            {
                { "ConfirmPassword", "Some Confirm Error" },
                { "PasswordResetCode", "Some Password Reset Error" }
            }));

            //Act
            var actual = await _accountOrchestrator.ResetPassword(new PasswordResetViewModel());

            //Assert
            Assert.AreEqual(string.Empty, actual.Password);
            Assert.AreEqual(string.Empty, actual.ConfirmPassword);
        }

        [Test]
        public async Task ThenTheUserIsNotLoggedInIfThereAreErrors()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<PasswordResetCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>
            {
                { "ConfirmPassword", "Some Confirm Error" },
                { "PasswordResetCode", "Some Password Reset Error" }
            }));

            //Act
            await _accountOrchestrator.ResetPassword(new PasswordResetViewModel());

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetUserByEmailAddressQuery>()), Times.Never);
            _owinWrapper.Verify(x => x.IssueLoginCookie(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheUserIsReturnedByEmailAddress()
        {
            //Arrange
            var actualResetCode = "123456";
            var model = new PasswordResetViewModel { Email = ValidEmail, PasswordResetCode = actualResetCode, Password = "password", ConfirmPassword = "passwordconfirm" };

            //Act
            await _accountOrchestrator.ResetPassword(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetUserByEmailAddressQuery>(c => c.EmailAddress == ValidEmail )), Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsLoggedInIfValid()
        {
            //Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "User"
            };
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserByEmailAddressQuery>())).ReturnsAsync(user);
            var model = new PasswordResetViewModel { Email = ValidEmail, PasswordResetCode = "123456", Password = "password", ConfirmPassword = "passwordconfirm" };

            //Act
            await _accountOrchestrator.ResetPassword(model);

            //Assert
            _owinWrapper.Verify(x => x.IssueLoginCookie(user.Id, $"{user.FirstName} {user.LastName}"), Times.Once);
        }
    }
}
