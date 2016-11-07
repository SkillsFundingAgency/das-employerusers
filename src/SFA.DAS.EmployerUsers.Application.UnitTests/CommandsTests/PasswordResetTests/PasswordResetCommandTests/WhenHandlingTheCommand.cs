using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerUsers.Application.Commands.PasswordReset;
using SFA.DAS.EmployerUsers.Application.Services.Notification;
using SFA.DAS.EmployerUsers.Application.Services.Password;
using SFA.DAS.EmployerUsers.Application.Validation;
using SFA.DAS.EmployerUsers.Domain;
using SFA.DAS.EmployerUsers.Domain.Data;

namespace SFA.DAS.EmployerUsers.Application.UnitTests.CommandsTests.PasswordResetTests.PasswordResetCommandTests
{
    public class WhenHandlingTheCommand
    {
        private Mock<IUserRepository> _userRepository;
        private Mock<IValidator<PasswordResetCommand>> _validator;
        private PasswordResetCommandHandler _passwordResetCommandHandler;
        private const string ActualEmailAddress = "someuser@local";
        public string PasswordResetCode = "123456ABC";
        private Mock<ICommunicationService> _communicationService;
        private Mock<IPasswordService> _passwordService;

        [SetUp]
        public void Arrange()
        {
            _communicationService = new Mock<ICommunicationService>();

            _passwordService = new Mock<IPasswordService>();
            _passwordService.Setup(x => x.GenerateAsync(It.IsAny<string>())).ReturnsAsync(new SecuredPassword());

            _userRepository = new Mock<IUserRepository>();
            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>())).ReturnsAsync(null);
            _userRepository.Setup(x => x.GetByEmailAddress(ActualEmailAddress)).ReturnsAsync(new User { Email = ActualEmailAddress, PasswordResetCode = PasswordResetCode, IsActive = true });

            _validator = new Mock<IValidator<PasswordResetCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<PasswordResetCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _passwordResetCommandHandler = new PasswordResetCommandHandler(_userRepository.Object, _validator.Object, _communicationService.Object, _passwordService.Object);
        }

        [Test]
        public async Task ThenTheUserIsReturnedFromTheRespository()
        {
            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress });

            //Assert
            _userRepository.Verify(x => x.GetByEmailAddress(ActualEmailAddress));

        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress });

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<PasswordResetCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheMessageIsPopulatedwithUserAndValidated()
        {
            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress });

            //Assert
            _validator.Verify(x => x.Validate(It.Is<PasswordResetCommand>(c => c.User != null)), Times.Once);
        }

        [Test]
        public async Task ThenTheUserIsUpdatedIfTheValidatorIsValid()
        {
            //Arrange
            _passwordService.Setup(x => x.GenerateAsync("somePassword")).ReturnsAsync(new SecuredPassword { HashedPassword = "hashedPassword", ProfileId = "theprofile", Salt = "salt" });

            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress, Password = "somePassword", ConfirmPassword = "someConfirmPassword" });

            //Assert
            _passwordService.Verify(x => x.GenerateAsync("somePassword"), Times.Once);
            _userRepository.Verify(x => x.Update(It.Is<User>(c => c.Email == ActualEmailAddress
                                                               && c.Password == "hashedPassword"
                                                               && c.Salt == "salt"
                                                               && c.PasswordProfileId == "theprofile"
                                                               && c.IsActive)), Times.Once);
        }

        [Test]
        public async Task ThenTheUsersAccessCodesAreExpiredIfTheValidatorIsValid()
        {
            //Arrange
            _passwordService.Setup(x => x.GenerateAsync("somePassword")).ReturnsAsync(new SecuredPassword { HashedPassword = "hashedPassword", ProfileId = "theprofile", Salt = "salt" });
            _userRepository.Setup(x => x.GetByEmailAddress(It.IsAny<string>())).ReturnsAsync(new User
            {
                Id = "USER1",
                Email = ActualEmailAddress,
                PasswordResetCode = PasswordResetCode,
                IsActive = true,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "111111",
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "222222",
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "333333",
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "444444",
                        CodeType = SecurityCodeType.UnlockCode,
                        ExpiryTime = DateTime.MaxValue
                    }
                }
            });

            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress, Password = "somePassword", ConfirmPassword = "someConfirmPassword" });

            //Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.Id == "USER1"
                                                               && !u.SecurityCodes.Any(sc => sc.CodeType == SecurityCodeType.AccessCode))),
                Times.Once);
        }

        [Test]
        public async Task ThenTheUsersPasswordResetCodesAreExpiredIfTheValidatorIsValid()
        {
            //Arrange
            _passwordService.Setup(x => x.GenerateAsync("somePassword")).ReturnsAsync(new SecuredPassword { HashedPassword = "hashedPassword", ProfileId = "theprofile", Salt = "salt" });
            _userRepository.Setup(x => x.GetByEmailAddress(ActualEmailAddress)).ReturnsAsync(new User
            {
                Id = "USER1",
                Email = ActualEmailAddress,
                PasswordResetCode = PasswordResetCode,
                IsActive = true,
                SecurityCodes = new[]
                {
                    new SecurityCode
                    {
                        Code = "111111",
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "222222",
                        CodeType = SecurityCodeType.PasswordResetCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "333333",
                        CodeType = SecurityCodeType.AccessCode,
                        ExpiryTime = DateTime.MaxValue
                    },
                    new SecurityCode
                    {
                        Code = "444444",
                        CodeType = SecurityCodeType.UnlockCode,
                        ExpiryTime = DateTime.MaxValue
                    }
                }
            });

            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress, Password = "somePassword", ConfirmPassword = "someConfirmPassword" });

            //Assert
            _userRepository.Verify(r => r.Update(It.Is<User>(u => u.Id == "USER1"
                                                               && !u.SecurityCodes.Any(sc => sc.CodeType == SecurityCodeType.PasswordResetCode))),
                Times.Once);
        }

        [Test]
        public void ThenAInvliadRequestExceptionIsThrownIfTheMessageIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<PasswordResetCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _passwordResetCommandHandler.Handle(new PasswordResetCommand()));

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void ThenTheUserIsNotUpdatedIfTheValidatorIsInValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<PasswordResetCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = "someotheremail@local" }));

            //Assert
            _userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        }


        [Test]
        public void ThenTheUserIsNotEmailedIfTheValidatorIsInValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<PasswordResetCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = "someotheremail@local" }));

            //Assert
            _communicationService.Verify(x => x.SendPasswordResetConfirmationMessage(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ThenAnEmailIsSentConfirmingThePasswordHasBeenRestWhenValid()
        {
            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress, Password = "somePassword", ConfirmPassword = "someConfirmPassword" });

            //Assert
            _communicationService.Verify(x => x.SendPasswordResetConfirmationMessage(It.Is<User>(c => c.Email == ActualEmailAddress), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ThenAnEmailIsSentToConfirmAccountActiviationIfItWasNotActive()
        {
            //Arrange
            _userRepository.Setup(x => x.GetByEmailAddress(ActualEmailAddress)).ReturnsAsync(new User { Email = ActualEmailAddress, PasswordResetCode = PasswordResetCode, IsActive = false });

            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress, Password = "somePassword", ConfirmPassword = "someConfirmPassword" });

            //Assert
            _communicationService.Verify(x => x.SendUserAccountConfirmationMessage(It.Is<User>(c => c.Email == ActualEmailAddress), It.IsAny<string>()), Times.Once);
        }


        [Test]
        public async Task ThenAnEmailIsNotSentToConfirmAccountActiviationIfItWasActive()
        {
            //Act
            await _passwordResetCommandHandler.Handle(new PasswordResetCommand { Email = ActualEmailAddress, Password = "somePassword", ConfirmPassword = "someConfirmPassword" });

            //Assert
            _communicationService.Verify(x => x.SendUserAccountConfirmationMessage(It.Is<User>(c => c.Email == ActualEmailAddress), It.IsAny<string>()), Times.Never);
        }
    }
}
