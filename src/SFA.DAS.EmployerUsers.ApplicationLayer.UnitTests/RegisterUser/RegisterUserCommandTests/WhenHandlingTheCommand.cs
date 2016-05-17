using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.ApplicationLayer.Commands.Validators;
using SFA.DAS.EmployerUsers.Data.User;
using SFA.DAS.EmployerUsers.Domain;

namespace SFA.DAS.EmployerUsers.ApplicationLayer.UnitTests.RegisterUser.RegisterUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private RegisterUserCommandHandler _registerUserCommandHandler;
        private Mock<IValidator<RegisterUserCommand>> _registerUserCommandValidator;
        private Mock<IUserRepository> _userRepository;

        [SetUp]
        public void Arrange()
        {
            _registerUserCommandValidator = new Mock<IValidator<RegisterUserCommand>>();
            _userRepository = new Mock<IUserRepository>();
            _registerUserCommandHandler = new RegisterUserCommandHandler(_registerUserCommandValidator.Object, _userRepository.Object);
        }

        [Test]
        public void ThenTheCommandIsHandledAndValidatorCalled()
        {
            //Act
            _registerUserCommandHandler.Handle(new RegisterUserCommand());

            //Assert
            _registerUserCommandValidator.Verify(x=>x.Validate(It.IsAny<RegisterUserCommand>()));
        }

        [Test]
        public void ThenTheUserIsCreatedIfTheCommandIsValid()
        {
            //Arrange
            var firstName = "Test";
            var lastName = "Tester";
            var emailAddress = "test@test.com";
            var password = "password";
            var registerUserCommand = new RegisterUserCommand
            {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = emailAddress,
                    Password = password
            };
            _registerUserCommandValidator.Setup(x => x.Validate(registerUserCommand)).Returns(true);

            //Act
            _registerUserCommandHandler.Handle(registerUserCommand);

            //Assert
            _userRepository.Verify(v=>v.Create(It.Is<User>(x=> x.FirstName.Equals(firstName) && x.LastName.Equals(lastName) && x.Email.Equals(emailAddress) && x.Password.Equals(password))));
        }

        [Test]
        public void ThenTheUserIsNotCreatedIfTheCommandIsInvalid()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(It.IsAny<RegisterUserCommand>())).Returns(false);
            
            //Act
            _registerUserCommandHandler.Handle(new RegisterUserCommand());

            //Assert
            _userRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Never);
        }
    }
}
