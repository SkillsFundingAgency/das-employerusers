using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Web.Authentication;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.OrchestratorTests.AccountOrchestratorTests
{
    public class WhenRegistering
    {
        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<IOwinWrapper> _owinWrapper;
        private RegisterViewModel _registerUserViewModel;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _owinWrapper = new Mock<IOwinWrapper>();

            _accountOrchestrator = new AccountOrchestrator(_mediator.Object, _owinWrapper.Object);

            _registerUserViewModel = new RegisterViewModel
            {
                FirstName = "test",
                LastName = "tester",
                Email = "test@test.com",
                Password = "password",
                ConfirmPassword = "password",
                HasAcceptedTermsAndConditions = true
            };
        }

        [Test]
        public async Task ThenABooleanValueIsReturned()
        {
            //Act
            var actual = await _accountOrchestrator.Register(_registerUserViewModel);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<RegisterViewModel>(actual);
        }

        [Test]
        public async Task ThenTheRegisterUserCommandIsPassedOntoTheMediator()
        {
            //Act
            var actual = await _accountOrchestrator.Register(_registerUserViewModel);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<RegisterUserCommand>(p => !string.IsNullOrEmpty(p.Id) && p.Email.Equals(_registerUserViewModel.Email) && p.FirstName.Equals(_registerUserViewModel.FirstName) && p.LastName.Equals(_registerUserViewModel.LastName) && p.Password.Equals(_registerUserViewModel.Password) && p.ConfirmPassword.Equals(_registerUserViewModel.ConfirmPassword) && p.HasAcceptedTermsAndConditions==_registerUserViewModel.HasAcceptedTermsAndConditions)), Times.Once);
            Assert.IsTrue(actual.Valid);
        }

        [Test]
        public async Task ThenTheUserIsSignedInOnSuccessfulRegistration()
        {
            //Act
            await _accountOrchestrator.Register(_registerUserViewModel);

            //Assert
            _owinWrapper.Verify(x => x.IssueLoginCookie(It.IsAny<string>(), "test tester"), Times.Once);
        }

        [Test]
        public async Task ThenFalseIsReturnedWhenTheRegisterUserCommandHandlerThrowsAnException()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RegisterUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "FirstName", "Some Error" } }));

            //Act
            var actual = await _accountOrchestrator.Register(_registerUserViewModel);

            //Assert
            Assert.IsFalse(actual.Valid);

        }

        [Test]
        public async Task ThenTheErrorsAreReturnedInTheException()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RegisterUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> { { "FirstName", "Some Error" } }));

            //Act
            var actual = await _accountOrchestrator.Register(_registerUserViewModel);

            //Assert
            Assert.IsAssignableFrom<RegisterViewModel>(actual);
            Assert.Contains(new KeyValuePair<string, string>("FirstName", "Some Error"), actual.ErrorDictionary);
            Assert.AreEqual("Some Error", actual.FirstNameError);
        }

        [Test]
        public async Task ThenTheErrorsAreCorrectlyMappedWhenAllFieldsHaveFailedValidation()
        {
            //Arrange
            var firstNameError = "Fist Name Error";
            var lastNameError = "Last Name Error";
            var emailError = "Email Error";
            var passwordError = "Password Error";
            var confirmPasswordError = "Confirm Password Error";
            _mediator.Setup(x => x.SendAsync(It.IsAny<RegisterUserCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>
            {
                { "FirstName", firstNameError },
                { "LastName", lastNameError },
                { "Email", emailError },
                { "Password", passwordError },
                { "ConfirmPassword", confirmPasswordError }
                
            }));

            //Act
            var actual = await _accountOrchestrator.Register(_registerUserViewModel);

            //Assert
            Assert.AreEqual(firstNameError, actual.FirstNameError);
            Assert.AreEqual(lastNameError, actual.LastNameError);
            Assert.AreEqual(emailError, actual.EmailError);
            Assert.AreEqual(passwordError, actual.PasswordError);
            Assert.AreEqual(confirmPasswordError, actual.ConfirmPasswordError);
        }

    }
}
