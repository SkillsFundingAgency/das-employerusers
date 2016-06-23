using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CodeGenerator;
using SFA.DAS.EmployerUsers.Application.Commands.RegisterUser;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.RegisterUserTests.RegisterUserCommandTests
{
    public class WhenHandlingTheCommand
    {
        private const string AccessCode = "ABC123XYZ";

        private Mock<IValidator<RegisterUserCommand>> _registerUserCommandValidator;
        private Mock<IUserRepository> _userRepository;
        private Mock<IPasswordService> _passwordService;
        private Mock<ICommunicationService> _communicationService;
        private Mock<ICodeGenerator> _codeGenerator;
        private RegisterUserCommandHandler _registerUserCommandHandler;
        private RegisterUserCommand _registerUserCommand;
        private SecuredPassword _securedPassword;

        [SetUp]
        public void Arrange()
        {
            _registerUserCommandValidator = new Mock<IValidator<RegisterUserCommand>>();

            _securedPassword = new SecuredPassword
            {
                HashedPassword = "Secured_Password",
                Salt = "Generated_Salt",
                ProfileId = "Password_Profile_Id"
            };
            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(s => s.GenerateAsync(It.IsAny<string>())).Returns(Task.FromResult(_securedPassword));

            _userRepository = new Mock<IUserRepository>();
            _communicationService = new Mock<ICommunicationService>();
            _codeGenerator = new Mock<ICodeGenerator>();
            _codeGenerator.Setup(x => x.GenerateAlphaNumeric(6)).Returns(AccessCode);
            _registerUserCommandHandler = new RegisterUserCommandHandler(_registerUserCommandValidator.Object, _passwordService.Object, _userRepository.Object,_communicationService.Object, _codeGenerator.Object);

            _registerUserCommand = new RegisterUserCommand
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Unit",
                LastName = "Tests",
                Email = "unit.tests@test.local",
                Password = "SomePassword",
                ConfirmPassword = "SomePassword"
            };
        }
        
        [Test]
        public void ThenTheCommandIsHandledAndValidatorCalled()
        {
            //Arrange
            const string errorKey = "MyError";
            const string errorMessage = "Some error has happened";

            _registerUserCommandValidator.Setup(x => x.Validate(It.IsAny<RegisterUserCommand>())).Returns(BuildValidationResult(errorKey, errorMessage));

            //Act
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));

            //Assert
            _registerUserCommandValidator.Verify(x=>x.Validate(It.IsAny<RegisterUserCommand>()));
            Assert.Contains(new KeyValuePair<string,string>(errorKey, errorMessage),actual.ErrorMessages);
        }

        [Test]
        public async Task ThenTheUserIsCreatedIfTheCommandIsValid()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(_registerUserCommand)).Returns(SuccessfullValidationResult);

            //Act
            await _registerUserCommandHandler.Handle(_registerUserCommand);

            //Assert
            _userRepository.Verify(v=>v.Create(It.Is<User>(x=> x.FirstName.Equals(_registerUserCommand.FirstName) && x.LastName.Equals(_registerUserCommand.LastName) && x.Email.Equals(_registerUserCommand.Email) && !x.Password.Equals(_registerUserCommand.Password) && x.AccessCode.Equals(AccessCode, StringComparison.InvariantCultureIgnoreCase))));
        }

        [Test]
        
        public void ThenTheUserIsNotCreatedIfTheCommandIsInvalid()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(It.IsAny<RegisterUserCommand>())).Returns(BuildValidationResult("", ""));

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));
            
            //Assert
            _userRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void ThenAnExceptionIsThrownIfTheCommandIsInvalid()
        {
            //Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(It.IsAny<RegisterUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });


            Assert.ThrowsAsync<InvalidRequestException>(async ()=> await _registerUserCommandHandler.Handle(new RegisterUserCommand()));
        }

        [Test]
        public async Task ThenTheUserIsCreatedWithSecurePasswordDetails()
        {
            // Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(_registerUserCommand)).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            // Act
            await _registerUserCommandHandler.Handle(_registerUserCommand);

            // Assert
            _userRepository.Verify(r => r.Create(It.Is<User>(u => u.Password == _securedPassword.HashedPassword 
                                                               && u.Salt == _securedPassword.Salt
                                                               && u.PasswordProfileId == _securedPassword.ProfileId)));
        }

        [Test]
        public async Task ThenTheCommunicationServiceIsCalledOnSuccessfulCommand()
        {
            // Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(_registerUserCommand)).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _registerUserCommandHandler.Handle(_registerUserCommand);

            //Assert
            _communicationService.Verify(x=>x.SendUserRegistrationMessage(It.Is<User>(m => m.Email == _registerUserCommand.Email), It.IsAny<string>()),Times.Once);
        }


        [Test]
        public async Task ThenTheAccessCodeIsSentToTheUserOnSuccessfulCreation()
        {
            // Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(_registerUserCommand)).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            await _registerUserCommandHandler.Handle(_registerUserCommand);

            //Assert
            _communicationService.Verify(x => x.SendUserRegistrationMessage(It.Is<User>(s=>s.AccessCode.Equals(AccessCode) && s.Id.Equals(_registerUserCommand.Id)),It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ThenTheAccessCodeIsNotSentToTheUserOnUnSuccessfulCreation()
        {
            // Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(It.IsAny<RegisterUserCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } }});

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(new RegisterUserCommand()));
            
            //Assert
            _communicationService.Verify(x => x.SendUserRegistrationMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenTheAccessCodeIsProvidedFromTheCodeGenerator()
        {
            // Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(_registerUserCommand)).Returns(SuccessfullValidationResult);

            //Act
            await _registerUserCommandHandler.Handle(_registerUserCommand);

            //Assert
            _codeGenerator.Verify(x=>x.GenerateAlphaNumeric(6),Times.Once);
        }

        [Test]
        public void ThenTheEmailAddressIsAlreadyActive()
        {
            // Arrange
            _registerUserCommandValidator.Setup(x => x.Validate(_registerUserCommand)).Returns(SuccessfullValidationResult);

            _userRepository.Setup(x => x.GetByEmailAddress(_registerUserCommand.Email)).ReturnsAsync(new User
            {
                Email = _registerUserCommand.Email,
                IsActive = true
            });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _registerUserCommandHandler.Handle(_registerUserCommand));
        }

        private ValidationResult BuildValidationResult(string errorKey, string errorMessage)
        {
            return new ValidationResult
            {
                ValidationDictionary = new Dictionary<string, string> {{errorKey, errorMessage}}
            };
        }

        private ValidationResult SuccessfullValidationResult => new ValidationResult {ValidationDictionary = new Dictionary<string, string>()};
    }
}
