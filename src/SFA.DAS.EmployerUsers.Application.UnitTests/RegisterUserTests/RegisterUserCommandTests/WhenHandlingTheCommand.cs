using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.RegisterUserTests.RegisterUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private RegisterUserCommandHandler _registerUserCommandHandler;
        private Mock<IValidator<RegisterUserCommand>> _registerUserCommandValidator;
        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordService> _passwordService;

        [SetUp]
        public void Arrange()
        {
            _registerUserCommandValidator = new Mock<IValidator<RegisterUserCommand>>();

            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(s => s.GenerateAsync(It.IsAny<string>())).Returns(Task.FromResult(new SecuredPassword
            {
                HashedPassword = "Secured_Password",
                Salt = "Generated_Salt",
                ProfileId = "Password_Profile_Id"
            }));

            _userRepository = new Mock<IUserRepository>();

            _registerUserCommandHandler = new RegisterUserCommandHandler(_registerUserCommandValidator.Object, _passwordService.Object, _userRepository.Object);
        }

        [Test]
        public void ThenTheCommandIsHandledAndValidatorCalled()
        {
            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));

            //Assert
            _registerUserCommandValidator.Verify(x=>x.Validate(It.IsAny<RegisterUserCommand>()));
        }

        [Test]
        public async Task ThenTheUserIsCreatedIfTheCommandIsValid()
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
                    Password = password,
                    ConfirmPassword = password
            };
            _registerUserCommandValidator.Setup(x => x.Validate(registerUserCommand)).Returns(true);

            //Act
            await _registerUserCommandHandler.Handle(registerUserCommand);

            //Assert
            _userRepository.Verify(v=>v.Create(It.Is<User>(x=> x.FirstName.Equals(firstName) && x.LastName.Equals(lastName) && x.Email.Equals(emailAddress) && !x.Password.Equals(password) && x.AccessCode.Equals("ABC123ZXY",StringComparison.InvariantCultureIgnoreCase))));
        }

        [Test]
        
        public void ThenTheUserIsNotCreatedIfTheCommandIsInvalid()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(It.IsAny<RegisterUserCommand>())).Returns(false);

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));
            
            //Assert
            _userRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void ThenAnExceptionIsThrownIfTheCommandIsInvalid()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(It.IsAny<RegisterUserCommand>())).Returns(false);


            Assert.ThrowsAsync<InvalidRequestException>(async ()=> await _registerUserCommandHandler.Handle(new RegisterUserCommand()));
        }

        [Test]
        public async Task ThenTheUserIsCreatedWithSecurePasswordDetails()
        {
            // Arrange
            var registerUserCommand = new RegisterUserCommand
            {
                FirstName = "Unit",
                LastName = "Tests",
                Email = "unit.tests@test.local",
                Password = "SomePassword",
                ConfirmPassword = "SomePassword"
            };
            _registerUserCommandValidator.Setup(x => x.Validate(registerUserCommand)).Returns(true);

            // Act
            await _registerUserCommandHandler.Handle(registerUserCommand);

            // Assert
            _userRepository.Verify(r => r.Create(It.Is<User>(u => u.Password == "Secured_Password" 
                                                               && u.Salt == "Generated_Salt"
                                                               && u.PasswordProfileId == "Password_Profile_Id")));
        }
        
    }
}
