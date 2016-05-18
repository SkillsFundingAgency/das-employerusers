using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Web.Models;
using SFA.DAS.EmployerUsers.Web.Orchestrators.Account;

namespace SFA.DAS.EmployerUsers.Web.UnitTests.AccountOrchestratorTests
{
    public class WhenRegistering
    {
        private AccountOrchestrator _accountOrchestrator;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _accountOrchestrator = new AccountOrchestrator(_mediator.Object);
        }

        [Test]
        public void ThenTheConfirmationViewModelIsReturned()
        {
            //Arrange
            var registerUserViewModel = new RegisterViewModel();

            //Act
            var actual = _accountOrchestrator.Register(registerUserViewModel);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<ConfirmationViewModel>(actual);
        }

        [Test]
        public void ThenTheRegisterUserCommandIsPassedOntoTheMediator()
        {
            //Arrange
            var confirmEmail = "test@test.com";
            var email = "test@test.com";
            var password = "password";
            var confirmPassword = "password";
            var lastName = "tester";
            var firstName = "test";

            var registerUserViewModel = new RegisterViewModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                ConfirmEmail = confirmEmail,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            //Act
            _accountOrchestrator.Register(registerUserViewModel);

            //Assert
            _mediator.Verify(x=>x.Send(It.Is<RegisterUserCommand>(p=>p.Email.Equals(email) && p.FirstName.Equals(firstName) && p.LastName.Equals(lastName) && p.Password.Equals(password) && p.ConfirmPassword.Equals(confirmPassword) && p.ConfirmEmail.Equals(confirmEmail))),Times.Once);
        }
        
    }
}
